# Architecture Documentation

## Table of Contents

- [System Overview](#system-overview)
- [Design Decisions](#design-decisions)
- [Dependency Graph](#dependency-graph)
- [Data Flow](#data-flow)
- [Service Responsibilities](#service-responsibilities)
- [Interface Contracts](#interface-contracts)
- [Configuration Binding](#configuration-binding)
- [Extension Patterns](#extension-patterns)

## System Overview

The Parking Management System follows **Clean Architecture** principles with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                       │
│                    (Program.cs - Console UI)                 │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                    Application Layer                         │
│              (ParkingManagerService - Orchestrator)         │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                    Domain Services Layer                    │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐    │
│  │  Layout      │  │  Pricing     │  │   Rules     │    │
│  │  Service     │  │  Service     │  │   Engine    │    │
│  └──────────────┘  └──────────────┘  └──────────────┘    │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                    Domain Models Layer                      │
│  ParkingLot, Floor, Section, Spot, Institution, Ticket    │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                    Infrastructure Layer                     │
│         (JsonParkingLayoutProvider - Data Access)          │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                    Configuration Layer                     │
│                    (parkingConfig.json)                     │
└─────────────────────────────────────────────────────────────┘
```

## Design Decisions

### 1. Why Interfaces Over Abstract Classes?

**Decision**: Use interfaces (`IParkingLayoutProvider`, `IPricingStrategy`, etc.) instead of abstract classes.

**Reasoning**:
- **Multiple inheritance**: C# classes can implement multiple interfaces but inherit from only one class
- **No shared implementation**: Different implementations (JSON, Database, API) share no common code
- **Contract-based**: We need contracts (what methods exist), not shared behavior
- **Testability**: Easier to mock interfaces for unit testing
- **Flexibility**: Can swap implementations without changing dependent code

**Example**:
```csharp
// Interface - defines contract
public interface IPricingStrategy
{
    decimal CalculateCharge(...);
}

// Multiple implementations possible
public class StandardPricingStrategy : IPricingStrategy { }
public class PeakHoursPricingStrategy : IPricingStrategy { }
public class DiscountPricingStrategy : IPricingStrategy { }
```

### 2. Why Separate JSON DTOs from Domain Models?

**Decision**: Create `JsonConfigModels` (DTOs) separate from domain models.

**Reasoning**:
- **Separation of concerns**: JSON structure can differ from domain structure
- **Versioning**: Can change JSON format without changing domain models
- **Mapping logic**: Centralized mapping ensures consistency
- **Future-proofing**: Easy to add new JSON fields without breaking domain models

**Example**:
```csharp
// JSON DTO (matches JSON structure)
public class InstitutionJson
{
    [JsonPropertyName("type")]
    public string Type { get; set; }  // String in JSON
}

// Domain Model (uses enum)
public class Institution
{
    public InstitutionType Type { get; set; }  // Enum in domain
}
```

### 3. Why Strategy Pattern for Pricing?

**Decision**: Use Strategy pattern (`IPricingStrategy`) instead of if/else chains.

**Reasoning**:
- **Open/Closed Principle**: Add new pricing strategies without modifying existing code
- **Single Responsibility**: Each strategy handles one pricing algorithm
- **Testability**: Test each strategy independently
- **Flexibility**: Can change pricing per institution at runtime

**Alternative (Not Used)**:
```csharp
// BAD: If/else chain
if (institution.Type == InstitutionType.Mall)
    return CalculateMallPricing(...);
else if (institution.Type == InstitutionType.Hospital)
    return CalculateHospitalPricing(...);
// Hard to extend, violates OCP
```

**Our Approach**:
```csharp
// GOOD: Strategy pattern
var strategy = GetStrategyForInstitution(institution);
return strategy.CalculateCharge(...);
// Easy to extend, follows OCP
```

### 4. Why Service Layer Instead of Direct Model Access?

**Decision**: Use services (`LayoutService`, `PricingService`) instead of direct model manipulation.

**Reasoning**:
- **Encapsulation**: Business logic is encapsulated in services
- **Reusability**: Services can be reused across different entry points
- **Testability**: Services can be tested independently
- **Consistency**: Ensures all operations follow same rules

**Example**:
```csharp
// BAD: Direct manipulation
spot.Status = AvailabilityStatus.Occupied;
spot.CurrentTicket = ticket;

// GOOD: Service method
_layoutService.AssignSpotToVehicle(spot, ticket);
// Service ensures consistency, validates state, etc.
```

### 5. Why Configuration-Driven?

**Decision**: All business rules come from JSON configuration, no hard-coded values.

**Reasoning**:
- **Flexibility**: Change rules without recompiling
- **Multi-tenancy**: Different configurations for different institutions
- **Testing**: Easy to test with different configurations
- **Database replacement**: JSON simulates database, easy to swap later

**Example**:
```csharp
// BAD: Hard-coded
if (institution.Type == InstitutionType.Mall)
    freeMinutes = 30;  // Hard-coded!

// GOOD: Configuration-driven
freeMinutes = institution.FreeMinutes;  // From JSON config
```

## Dependency Graph

```
Program.cs
    │
    ├──► ParkingManagerService
    │       │
    │       ├──► LayoutService
    │       │       └──► IParkingLayoutProvider (JsonParkingLayoutProvider)
    │       │
    │       ├──► PricingService
    │       │       └──► IPricingStrategy (StandardPricingStrategy)
    │       │
    │       ├──► IParkingRuleSet (InstitutionRuleEngine)
    │       │
    │       └──► IParkingLayoutProvider (for saving)
    │
    └──► Models (ParkingLot, Institution, etc.)
```

**Key Points**:
- All dependencies flow inward (Dependency Inversion Principle)
- High-level modules (Program) depend on abstractions (interfaces)
- Low-level modules (JsonParkingLayoutProvider) implement abstractions
- No circular dependencies

## Data Flow

### Vehicle Entry Flow

```
1. User Input (Program.cs)
   │
   ▼
2. ParkingManagerService.EnterParkingAsync()
   │
   ├──► Validate vehicle type (IParkingRuleSet)
   │
   ├──► Find available spot (LayoutService)
   │       │
   │       └──► Load parking lot (IParkingLayoutProvider)
   │       └──► Search floors/sections/spots
   │
   ├──► Generate ticket
   │
   ├──► Assign spot (LayoutService.AssignSpotToVehicle)
   │       │
   │       └──► Update spot.Status = Occupied
   │       └──► Set spot.CurrentTicket = ticket
   │
   └──► Return ticket to user
```

### Vehicle Exit Flow

```
1. User Input (Program.cs)
   │
   ▼
2. ParkingManagerService.ExitParkingAsync()
   │
   ├──► Find ticket
   │
   ├──► Set exit time
   │
   ├──► Validate duration (IParkingRuleSet)
   │
   ├──► Locate spot (LayoutService)
   │
   ├──► Calculate charge (PricingService)
   │       │
   │       └──► IPricingStrategy.CalculateCharge()
   │               │
   │               ├──► Check if free parking
   │               ├──► Calculate base rate
   │               ├──► Calculate hourly charges
   │               └──► Apply spot type multipliers
   │
   ├──► Free spot (LayoutService.FreeSpot)
   │       │
   │       └──► Update spot.Status = Available
   │       └──► Clear spot.CurrentTicket = null
   │
   └──► Return exit result with charge
```

## Service Responsibilities

### ParkingManagerService

**Responsibility**: Orchestrate parking operations (entry/exit)

**Dependencies**:
- `LayoutService` - For spot management
- `PricingService` - For charge calculation
- `IParkingRuleSet` - For rule validation
- `IParkingLayoutProvider` - For data persistence

**Key Methods**:
- `EnterParkingAsync()` - Complete entry flow
- `ExitParkingAsync()` - Complete exit flow
- `GetAvailabilityAsync()` - Get availability stats

**Why it exists**: Provides high-level API that coordinates multiple services.

### LayoutService

**Responsibility**: Manage parking layout and spot operations

**Dependencies**:
- `IParkingLayoutProvider` - For loading layout

**Key Methods**:
- `FindAvailableSpotAsync()` - Find suitable spot
- `AssignSpotToVehicle()` - Assign spot to vehicle
- `FreeSpot()` - Release spot
- `GetAvailabilityAsync()` - Get availability stats

**Why it exists**: Encapsulates complex spot-finding logic and ensures consistent state management.

### PricingService

**Responsibility**: Orchestrate pricing calculations

**Dependencies**:
- `IPricingStrategy` - For actual calculation

**Key Methods**:
- `CalculateCharge()` - Calculate total charge

**Why it exists**: Provides abstraction over pricing strategy, allows swapping strategies.

### InstitutionRuleEngine

**Responsibility**: Validate parking rules

**Dependencies**: None (pure logic)

**Key Methods**:
- `IsVehicleTypeAllowed()` - Validate vehicle type
- `CanVehicleParkInSpot()` - Validate spot eligibility
- `IsParkingDurationValid()` - Validate duration

**Why it exists**: Centralizes all rule validation logic in one place.

### JsonParkingLayoutProvider

**Responsibility**: Load/save parking configuration from JSON files

**Dependencies**: None (uses System.Text.Json)

**Key Methods**:
- `LoadParkingLotAsync()` - Deserialize JSON to domain models
- `SaveParkingLotAsync()` - Serialize domain models to JSON (future)

**Why it exists**: Implements data access abstraction, can be replaced with database provider.

## Interface Contracts

### IParkingManager

**Purpose**: Main contract for parking operations

**Implementations**: `ParkingManagerService`

**Key Design**:
- Async methods for I/O operations
- Returns nullable types for "not found" scenarios
- Uses DTOs (`ParkingAvailability`, `ParkingExitResult`) for return values

### IParkingLayoutProvider

**Purpose**: Abstract data access

**Implementations**: `JsonParkingLayoutProvider` (can add `DatabaseParkingLayoutProvider`)

**Key Design**:
- Async methods (supports both file and database I/O)
- Returns domain models (not DTOs)
- Separates read (`LoadParkingLotAsync`) from write (`SaveParkingLotAsync`)

### IPricingStrategy

**Purpose**: Abstract pricing calculation

**Implementations**: `StandardPricingStrategy` (can add `PeakHoursPricingStrategy`, etc.)

**Key Design**:
- Pure function (no side effects)
- Takes all needed data as parameters
- Returns decimal (simple, testable)

### IParkingRuleSet

**Purpose**: Abstract rule validation

**Implementations**: `InstitutionRuleEngine`

**Key Design**:
- Pure functions (no side effects)
- Boolean return values (simple, testable)
- Takes all needed data as parameters

## Configuration Binding

### JSON to Domain Model Mapping

```
parkingConfig.json
    │
    ▼
JsonConfigModels (DTOs)
    │
    ▼ (Mapping in JsonParkingLayoutProvider.MapToDomainModel())
    │
    ▼
Domain Models (ParkingLot, Institution, etc.)
```

### Mapping Process

1. **Deserialize JSON** → `ParkingConfigJson` (DTO)
2. **Parse enums** → Convert strings to enum types
3. **Map hierarchy** → Build nested structure (Lot → Floors → Sections → Spots)
4. **Create domain models** → Return `ParkingLot` with all relationships

### Why This Approach?

- **Type safety**: Domain models use enums, not strings
- **Validation**: Can validate during mapping
- **Flexibility**: JSON structure can evolve independently
- **Testability**: Can test mapping logic separately

## Extension Patterns

### Adding a New Pricing Strategy

```csharp
// 1. Create new strategy
public class DiscountPricingStrategy : IPricingStrategy
{
    public decimal CalculateCharge(ParkingTicket ticket, Institution institution, SpotType spotType)
    {
        // Custom logic with discounts
    }
}

// 2. Inject in Program.cs
var pricingStrategy = new DiscountPricingStrategy();
var pricingService = new PricingService(pricingStrategy);
```

### Adding a New Data Provider

```csharp
// 1. Implement interface
public class DatabaseParkingLayoutProvider : IParkingLayoutProvider
{
    public async Task<ParkingLot> LoadParkingLotAsync()
    {
        // Load from database
    }
    
    public async Task SaveParkingLotAsync(ParkingLot parkingLot)
    {
        // Save to database
    }
}

// 2. Replace in Program.cs
var layoutProvider = new DatabaseParkingLayoutProvider(connectionString);
```

### Adding a New Institution Type

```csharp
// 1. Add to enum
public enum InstitutionType
{
    Mall,
    Theatre,
    Hospital,
    College,
    Corporate,
    Airport  // NEW
}

// 2. Update JSON config
{
  "institution": {
    "type": "Airport",
    // ... rest of config
  }
}

// 3. Optionally create custom pricing strategy
public class AirportPricingStrategy : IPricingStrategy { }
```

## Testing Strategy

### Unit Testing

Each service can be tested independently by mocking dependencies:

```csharp
// Example: Test PricingService
var mockStrategy = new Mock<IPricingStrategy>();
var service = new PricingService(mockStrategy.Object);
// Test service logic
```

### Integration Testing

Test with real JSON configuration:

```csharp
// Load real config
var provider = new JsonParkingLayoutProvider("test-config.json");
var parkingLot = await provider.LoadParkingLotAsync();
// Verify structure
```

### End-to-End Testing

Test complete flows through `ParkingManagerService`:

```csharp
var manager = new ParkingManagerService(...);
var ticket = await manager.EnterParkingAsync(vehicle);
var result = await manager.ExitParkingAsync(ticket.TicketId);
// Verify complete flow
```

## Performance Considerations

### Current Implementation

- **In-memory**: All data loaded into memory
- **Synchronous operations**: File I/O is async but operations are sequential
- **No caching**: Configuration loaded on every request

### Future Optimizations

- **Caching**: Cache parking lot structure in memory
- **Lazy loading**: Load floors/sections on demand
- **Database**: Replace JSON with database for better performance
- **Concurrency**: Add locking for multi-threaded access

## Security Considerations

### Current Implementation

- **No authentication**: Console app, no user authentication
- **No authorization**: All operations allowed
- **File-based**: JSON files accessible to anyone with file system access

### Future Enhancements

- **API layer**: Add authentication/authorization
- **Encryption**: Encrypt sensitive data in configuration
- **Audit logging**: Log all parking operations
- **Input validation**: Validate all user inputs

---

This architecture is designed to be:
- **Maintainable**: Clear separation of concerns
- **Testable**: Dependencies are injected, interfaces are mockable
- **Extensible**: Easy to add new features without breaking existing code
- **Scalable**: Can replace components (JSON → Database) without major refactoring

