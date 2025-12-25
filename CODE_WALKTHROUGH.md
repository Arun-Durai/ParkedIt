# Code Walkthrough

This document provides a guided tour through the codebase, explaining key files and their purposes.

## Entry Point: Program.cs

**Location**: `/ParkedIt/Program.cs`

**Purpose**: Console application entry point and user interface.

**Key Sections**:

1. **Main Method**: Entry point that initializes services and starts menu loop
2. **InitializeServices()**: Sets up dependency injection manually
   - Creates `JsonParkingLayoutProvider` with config file path
   - Creates `InstitutionRuleEngine` for rule validation
   - Creates `StandardPricingStrategy` for pricing
   - Wires everything together
3. **Menu System**: Provides console UI for testing
   - Vehicle entry
   - Vehicle exit
   - Availability check
   - Active tickets view

**Why it's structured this way**: 
- Manual DI for console app (in production, use DI container)
- Menu system demonstrates all features
- Clear separation between UI and business logic

## Domain Models

### Enums.cs

**Location**: `/ParkedIt/Models/Enums.cs`

**Purpose**: Defines all enumeration types used throughout the system.

**Key Enums**:
- `InstitutionType`: Mall, Theatre, Hospital, College, Corporate
- `VehicleType`: Car, Bike, EV, Truck, Auto, Bicycle
- `SpotType`: Standard, VIP, Staff, Accessible
- `AvailabilityStatus`: Available, Occupied, Disabled, Reserved

**Why enums**: Type safety, prevents invalid values, IDE autocomplete.

### ParkingLot.cs

**Location**: `/ParkedIt/Models/ParkingLot.cs`

**Purpose**: Root model representing entire parking structure.

**Key Properties**:
- `Floors`: List of all floors
- `Institution`: Institution configuration
- `TotalCapacity`: Calculated property (sum of enabled spots)
- `OccupiedCount`: Calculated property (count of occupied spots)
- `AvailableCount`: Calculated property (capacity - occupied)

**Why calculated properties**: Always up-to-date, no need to manually track.

### Spot.cs

**Location**: `/ParkedIt/Models/Spot.cs`

**Purpose**: Represents individual parking spot.

**Key Properties**:
- `Type`: Spot type (Standard, VIP, etc.)
- `Status`: Current availability status
- `AllowedVehicleTypes`: Vehicle type restrictions
- `CurrentTicket`: Reference to active ticket (if occupied)
- `IsEnabled`: Whether spot is active

**Why CurrentTicket reference**: Enables bidirectional navigation (spot → ticket, ticket → spot).

### ParkingTicket.cs

**Location**: `/ParkedIt/Models/ParkingTicket.cs`

**Purpose**: Represents parking session.

**Key Properties**:
- `TicketId`: Unique identifier
- `Vehicle`: Vehicle information
- `EntryTime`: When vehicle entered
- `ExitTime`: When vehicle left (null if still parked)
- `SpotLocation`: Reference to spot location
- `TotalCharge`: Calculated charge

**Why SpotLocation (not Spot reference)**: Lightweight, doesn't require object graph navigation.

## Interfaces

### IParkingManager.cs

**Location**: `/ParkedIt/Interfaces/IParkingManager.cs`

**Purpose**: Main contract for parking operations.

**Key Methods**:
- `EnterParkingAsync()`: Process vehicle entry
- `ExitParkingAsync()`: Process vehicle exit
- `GetAvailabilityAsync()`: Get availability stats

**Why async**: Supports both file I/O (current) and database I/O (future).

**Return Types**:
- `Task<ParkingTicket?>`: Nullable for "not found" scenarios
- `Task<ParkingExitResult?>`: Encapsulates exit data
- `Task<ParkingAvailability>`: Encapsulates availability data

### IParkingLayoutProvider.cs

**Location**: `/ParkedIt/Interfaces/IParkingLayoutProvider.cs`

**Purpose**: Abstract data access layer.

**Key Methods**:
- `LoadParkingLotAsync()`: Load structure from source
- `SaveParkingLotAsync()`: Save structure to source

**Why separate interface**: Can swap JSON provider with database provider without changing dependent code.

### IPricingStrategy.cs

**Location**: `/ParkedIt/Interfaces/IPricingStrategy.cs`

**Purpose**: Abstract pricing calculation.

**Key Method**:
- `CalculateCharge()`: Calculate total charge

**Why strategy pattern**: Different institutions can have different pricing algorithms.

**Design**: Pure function - no side effects, easy to test.

### IParkingRuleSet.cs

**Location**: `/ParkedIt/Interfaces/IParkingRuleSet.cs`

**Purpose**: Abstract rule validation.

**Key Methods**:
- `IsVehicleTypeAllowed()`: Validate vehicle type
- `CanVehicleParkInSpot()`: Validate spot eligibility
- `IsParkingDurationValid()`: Validate duration

**Why separate interface**: Rules can vary by institution, easy to swap implementations.

## Services

### ParkingManagerService.cs

**Location**: `/ParkedIt/Services/ParkingManagerService.cs`

**Purpose**: Orchestrates parking operations.

**Dependencies**:
- `LayoutService`: For spot management
- `PricingService`: For charge calculation
- `IParkingRuleSet`: For rule validation
- `IParkingLayoutProvider`: For data persistence

**Key Flows**:

**Entry Flow**:
1. Validate vehicle type
2. Find available spot
3. Generate ticket
4. Assign spot
5. Store ticket

**Exit Flow**:
1. Find ticket
2. Set exit time
3. Validate duration
4. Locate spot
5. Calculate charge
6. Free spot
7. Return result

**Why it exists**: Provides high-level API that coordinates multiple services.

### LayoutService.cs

**Location**: `/ParkedIt/Services/LayoutService.cs`

**Purpose**: Manages parking layout and spot operations.

**Key Methods**:

**FindAvailableSpotAsync()**:
- Searches enabled floors
- Searches enabled sections
- Searches enabled spots
- Filters by vehicle type
- Filters by spot type (if preferred)
- Returns first match

**Why this algorithm**: 
- Prefers preferred spot type
- Falls back to any available spot
- Respects all enable/disable flags

**AssignSpotToVehicle()**:
- Sets spot status to Occupied
- Sets spot current ticket reference

**Why encapsulated**: Ensures consistent state updates.

**FreeSpot()**:
- Sets spot status to Available
- Clears spot current ticket reference

**Why encapsulated**: Ensures consistent cleanup.

### PricingService.cs

**Location**: `/ParkedIt/Services/PricingService.cs`

**Purpose**: Orchestrates pricing calculations.

**Key Method**:

**CalculateCharge()**:
- Checks if parking is free
- Delegates to strategy for calculation
- Returns total charge

**Why it exists**: Provides abstraction over strategy, allows swapping strategies.

### StandardPricingStrategy.cs

**Location**: `/ParkedIt/Services/StandardPricingStrategy.cs`

**Purpose**: Implements standard pricing algorithm.

**Calculation Logic**:
1. Start with base rate
2. Calculate billable minutes (total - free minutes)
3. Calculate billable hours (round up)
4. Add hourly charges
5. Apply spot type multiplier

**Why round up hours**: Standard parking practice - partial hours count as full hours.

**Spot Type Multipliers**:
- VIP: 1.5x (50% more)
- Accessible: 0.8x (20% less)
- Staff: 0x (free)
- Standard: 1.0x (no change)

**Why multipliers**: Allows different pricing for different spot types.

### InstitutionRuleEngine.cs

**Location**: `/ParkedIt/Services/InstitutionRuleEngine.cs`

**Purpose**: Validates parking rules.

**Key Validations**:

**IsVehicleTypeAllowed()**:
- Checks institution's allowed vehicle types
- Empty list = all types allowed

**CanVehicleParkInSpot()**:
- Checks spot is enabled
- Checks spot is available
- Checks vehicle type restrictions on spot
- Empty list = all types allowed

**IsParkingDurationValid()**:
- Calculates duration
- Compares to max parking hours
- 0 = unlimited

**Why pure functions**: No side effects, easy to test, predictable.

### JsonParkingLayoutProvider.cs

**Location**: `/ParkedIt/Services/JsonParkingLayoutProvider.cs`

**Purpose**: Loads parking configuration from JSON files.

**Key Process**:

1. **Read JSON file**: Uses `File.ReadAllTextAsync()`
2. **Deserialize**: Uses `JsonSerializer.Deserialize<ParkingConfigJson>()`
3. **Map to domain**: Converts JSON DTOs to domain models
4. **Parse enums**: Converts strings to enum types
5. **Build hierarchy**: Creates nested structure

**Why separate mapping**: 
- JSON structure can differ from domain structure
- Centralized mapping logic
- Easy to handle version changes

**Error Handling**:
- File not found → throws `FileNotFoundException`
- Invalid JSON → throws deserialization exception
- Invalid enum → throws `ArgumentException`

## Utilities

### JsonConfigModels.cs

**Location**: `/ParkedIt/Utils/JsonConfigModels.cs`

**Purpose**: Data Transfer Objects (DTOs) for JSON deserialization.

**Key Classes**:
- `ParkingConfigJson`: Root JSON structure
- `InstitutionJson`: Institution configuration
- `ParkingRulesJson`: Rules configuration
- `ParkingLotJson`: Parking lot structure
- `FloorJson`, `SectionJson`, `SpotJson`: Nested structure

**Why separate from domain models**:
- JSON structure can evolve independently
- Can handle missing/null fields gracefully
- Type conversion (string → enum) happens during mapping

**JsonPropertyName Attributes**: Maps JSON property names to C# properties.

## Configuration

### parkingConfig.json

**Location**: `/ParkedIt/Config/parkingConfig.json`

**Purpose**: Contains all business rules and parking structure.

**Structure**:
- `institution`: Institution configuration
- `parkingLot`: Parking structure (floors → sections → spots)

**Why JSON**:
- Human-readable
- Easy to edit
- No compilation needed
- Can be replaced with database later

**Key Features**:
- Multi-floor structure
- Different spot types
- Enable/disable flags
- Vehicle type restrictions

## Code Patterns

### Dependency Injection

**Pattern**: Constructor injection

**Example**:
```csharp
public class ParkingManagerService
{
    private readonly LayoutService _layoutService;
    
    public ParkingManagerService(LayoutService layoutService)
    {
        _layoutService = layoutService ?? throw new ArgumentNullException();
    }
}
```

**Why**: 
- Dependencies are explicit
- Easy to test (can inject mocks)
- Follows Dependency Inversion Principle

### Nullable Reference Types

**Pattern**: Nullable types for optional values

**Example**:
```csharp
public ParkingTicket? EnterParkingAsync(...)  // Can return null
public DateTime? ExitTime { get; set; }      // Can be null
```

**Why**: 
- Explicitly indicates "not found" or "not set"
- Compiler warnings for null checks
- Prevents null reference exceptions

### Async/Await

**Pattern**: Async methods for I/O operations

**Example**:
```csharp
public async Task<ParkingLot> LoadParkingLotAsync()
{
    var json = await File.ReadAllTextAsync(_configFilePath);
    // ...
}
```

**Why**: 
- Non-blocking I/O
- Better performance
- Supports both file and database I/O

### Calculated Properties

**Pattern**: Properties that compute values

**Example**:
```csharp
public int TotalCapacity => Floors
    .Where(f => f.IsEnabled)
    .SelectMany(f => f.Sections)
    .Where(s => s.IsEnabled)
    .SelectMany(s => s.Spots)
    .Count(s => s.IsEnabled);
```

**Why**: 
- Always up-to-date
- No manual tracking needed
- Single source of truth

## Testing Considerations

### What to Test

1. **Unit Tests**: Each service independently
2. **Integration Tests**: Services working together
3. **Configuration Tests**: Different JSON configurations
4. **Edge Cases**: No spots available, invalid tickets, etc.

### How to Test

**Mock Dependencies**:
```csharp
var mockProvider = new Mock<IParkingLayoutProvider>();
var service = new LayoutService(mockProvider.Object);
```

**Test with Real Config**:
```csharp
var provider = new JsonParkingLayoutProvider("test-config.json");
var parkingLot = await provider.LoadParkingLotAsync();
```

## Common Modifications

### Change Pricing Algorithm

1. Create new `IPricingStrategy` implementation
2. Update `Program.cs` to use new strategy
3. No other changes needed

### Add New Institution Type

1. Add to `InstitutionType` enum
2. Update JSON config
3. Optionally create custom pricing strategy

### Replace JSON with Database

1. Create `DatabaseParkingLayoutProvider` implementing `IParkingLayoutProvider`
2. Update `Program.cs` to use new provider
3. No other changes needed

---

This codebase is designed to be:
- **Readable**: Clear naming, good comments
- **Maintainable**: Separation of concerns, single responsibility
- **Testable**: Dependencies injected, interfaces mockable
- **Extensible**: Easy to add features without breaking existing code

