# Parking Management System

A production-quality, configuration-driven parking management system backend built with C# and .NET. This project demonstrates clean architecture, SOLID principles, and extensibility best practices.

## 📋 Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Key Features](#key-features)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Usage Guide](#usage-guide)
- [Design Principles](#design-principles)
- [Extension Points](#extension-points)
- [API Reference](#api-reference)

## 🎯 Overview

This Parking Management System is a learning-first but production-quality backend application that manages parking operations for various institution types (Malls, Theatres, Hospitals, Colleges, Corporate offices). The system is entirely configuration-driven, meaning all business rules, pricing, and parking structure come from JSON configuration files - no hard-coded values.

### Core Principles

- ✅ **Configuration-Driven**: All business logic, rules, and structure come from JSON files
- ✅ **Clean Architecture**: Clear separation of concerns with layers (Models, Interfaces, Services)
- ✅ **SOLID Principles**: Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion
- ✅ **Extensibility**: New vehicle types, spot types, and institution types can be added via configuration
- ✅ **Dependency Inversion**: Services depend on abstractions (interfaces), not concrete implementations

## 🏗 Architecture

### Architectural Layers

```
┌─────────────────────────────────────┐
│         Program.cs (Entry Point)     │
└─────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────┐
│         Services Layer               │
│  - ParkingManagerService            │
│  - LayoutService                    │
│  - PricingService                   │
│  - InstitutionRuleEngine            │
└─────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────┐
│         Interfaces Layer             │
│  - IParkingManager                  │
│  - IParkingLayoutProvider           │
│  - IPricingStrategy                 │
│  - IParkingRuleSet                  │
└─────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────┐
│         Models Layer                 │
│  - ParkingLot, Floor, Section, Spot │
│  - Institution, ParkingTicket       │
│  - VehicleInfo, PricingConfig       │
└─────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────┐
│      Configuration (JSON)            │
│      - parkingConfig.json            │
└─────────────────────────────────────┘
```

### Design Patterns Used

1. **Strategy Pattern**: `IPricingStrategy` allows different pricing algorithms per institution
2. **Dependency Injection**: Services receive dependencies via constructor injection
3. **Facade Pattern**: `ParkingManagerService` provides simple interface to complex subsystem
4. **Repository Pattern**: `IParkingLayoutProvider` abstracts data access

## 📁 Project Structure

```
ParkedIt/
├── Config/
│   └── parkingConfig.json          # Main configuration file
├── Models/                          # Domain models
│   ├── Enums.cs                    # InstitutionType, VehicleType, SpotType, AvailabilityStatus
│   ├── ParkingLot.cs               # Root parking structure model
│   ├── Floor.cs                    # Floor model
│   ├── Section.cs                  # Section model
│   ├── Spot.cs                     # Individual parking spot
│   ├── Institution.cs              # Institution configuration
│   ├── ParkingRules.cs             # Institution-specific rules
│   ├── ParkingTicket.cs            # Parking session ticket
│   ├── VehicleInfo.cs              # Vehicle information
│   └── PricingConfig.cs             # Pricing configuration
├── Interfaces/                      # Service contracts
│   ├── IParkingLayoutProvider.cs   # Data access abstraction
│   ├── IParkingManager.cs          # Parking operations contract
│   ├── IPricingStrategy.cs         # Pricing calculation strategy
│   └── IParkingRuleSet.cs          # Rule validation contract
├── Services/                        # Business logic implementations
│   ├── ParkingManagerService.cs    # Main orchestrator
│   ├── LayoutService.cs            # Layout and spot management
│   ├── PricingService.cs           # Pricing orchestration
│   ├── StandardPricingStrategy.cs   # Standard pricing implementation
│   ├── InstitutionRuleEngine.cs    # Rule validation engine
│   └── JsonParkingLayoutProvider.cs # JSON file data provider
├── Utils/                           # Utility classes
│   └── JsonConfigModels.cs         # JSON DTOs for configuration binding
├── Program.cs                       # Console application entry point
└── ParkedIt.csproj                 # Project file
```

## ✨ Key Features

### 1. Multi-Institution Support
- **Mall**: Standard parking with hourly rates
- **Theatre**: Short-term parking
- **Hospital**: Extended parking with different rules
- **College**: Student/staff parking
- **Corporate**: Employee parking

### 2. Dynamic Parking Structure
- Any number of floors
- Any number of sections per floor
- Any number of spots per section
- Enable/disable floors, sections, or individual spots

### 3. Flexible Vehicle Types
- Car, Bike, EV, Truck, Auto, Bicycle
- Extensible via configuration

### 4. Spot Types
- **Standard**: Regular parking
- **VIP**: Premium parking (higher rates)
- **Staff**: Staff-only parking (often free)
- **Accessible**: Accessible parking (discounted rates)

### 5. Intelligent Pricing
- Base rate (one-time entry fee)
- Hourly rate (after free minutes)
- Free minutes (configurable per institution)
- Spot type multipliers (VIP costs more, Accessible costs less)
- Institution-specific pricing rules

### 6. Rule Validation
- Vehicle type restrictions
- Spot type access control
- Maximum parking duration
- Institution-specific constraints

## 🚀 Getting Started

### Prerequisites

- .NET 10.0 SDK or later
- A code editor (Visual Studio, VS Code, Rider, etc.)

### Installation

1. **Clone or navigate to the project directory:**
   ```bash
   cd /Users/username/Projects/ParkedIt/ParkedIt
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Build the project:**
   ```bash
   dotnet build
   ```

4. **Run the application:**
   ```bash
   dotnet run
   ```

### Troubleshooting

If you encounter SDK permission issues:
```bash
sudo chown -R $(whoami):staff /usr/local/share/dotnet
sudo chmod -R u+w /usr/local/share/dotnet
```

## ⚙️ Configuration

### Configuration File Structure

The system reads from `Config/parkingConfig.json`. Here's the structure:

```json
{
  "institution": {
    "id": "INST-001",
    "name": "Grand Shopping Mall",
    "type": "Mall",
    "isFreeParking": false,
    "freeMinutes": 30,
    "baseRate": 10.00,
    "hourlyRate": 5.00,
    "rules": {
      "maxParkingHours": 8,
      "hasVipSpots": true,
      "hasStaffSpots": true,
      "hasAccessibleSpots": true,
      "allowedVehicleTypes": ["Car", "Bike", "EV", "Auto", "Bicycle"],
      "timeBasedRules": {}
    }
  },
  "parkingLot": {
    "id": "LOT-001",
    "name": "Grand Mall Parking",
    "floors": [
      {
        "id": "FLOOR-1",
        "name": "Ground Floor",
        "isEnabled": true,
        "sections": [
          {
            "id": "SEC-A",
            "name": "Section A",
            "isEnabled": true,
            "spots": [
              {
                "id": "SPOT-001",
                "type": "VIP",
                "status": "Available",
                "isEnabled": true,
                "allowedVehicleTypes": []
              }
            ]
          }
        ]
      }
    ]
  }
}
```

### Configuration Fields

#### Institution Configuration

| Field | Type | Description |
|-------|------|-------------|
| `id` | string | Unique institution identifier |
| `name` | string | Display name |
| `type` | string | Institution type (Mall, Theatre, Hospital, College, Corporate) |
| `isFreeParking` | boolean | Whether parking is completely free |
| `freeMinutes` | int | Free minutes before billing starts |
| `baseRate` | decimal | One-time entry fee |
| `hourlyRate` | decimal | Hourly rate after free minutes |

#### Parking Rules

| Field | Type | Description |
|-------|------|-------------|
| `maxParkingHours` | int | Maximum allowed parking duration (0 = unlimited) |
| `hasVipSpots` | boolean | Whether VIP spots exist |
| `hasStaffSpots` | boolean | Whether staff spots exist |
| `hasAccessibleSpots` | boolean | Whether accessible spots exist |
| `allowedVehicleTypes` | string[] | Vehicle types allowed (empty = all allowed) |

#### Parking Structure

- **Floors**: Top-level structure
  - `isEnabled`: Can disable entire floors
- **Sections**: Groups of spots within a floor
  - `isEnabled`: Can disable entire sections
- **Spots**: Individual parking spaces
  - `type`: Standard, VIP, Staff, Accessible
  - `status`: Available, Occupied, Disabled, Reserved
  - `isEnabled`: Can disable individual spots
  - `allowedVehicleTypes`: Vehicle type restrictions (empty = all allowed)

## 📖 Usage Guide

### Running the Application

When you run `dotnet run`, you'll see a console menu:

```
========================================
   Parking Management System
   Configuration-Driven Backend
========================================

✅ Services initialized successfully

📊 Parking Lot Status:
   Total Capacity: 20 spots
   Available: 20 spots
   Occupied: 0 spots

========================================
   Main Menu
========================================
1. Vehicle Entry
2. Vehicle Exit
3. Check Availability
4. View Active Tickets
5. Exit Application
```

### Vehicle Entry Flow

1. Select option **1** (Vehicle Entry)
2. Enter license plate number
3. Select vehicle type (Car, Bike, EV, etc.)
4. Optionally select preferred spot type (VIP, Accessible, etc.)
5. System finds available spot and generates ticket
6. Ticket ID and spot location are displayed

### Vehicle Exit Flow

1. Select option **2** (Vehicle Exit)
2. Enter ticket ID or select from active tickets list
3. System calculates parking duration and charges
4. Spot is freed and ticket is closed
5. Total charge is displayed

### Checking Availability

Select option **3** to see:
- Total capacity
- Available spots
- Occupied spots
- Breakdown by spot type

## 🎓 Design Principles

### SOLID Principles Applied

1. **Single Responsibility Principle (SRP)**
   - Each service has one clear responsibility
   - `LayoutService` only handles layout operations
   - `PricingService` only handles pricing calculations

2. **Open/Closed Principle (OCP)**
   - Open for extension (new pricing strategies, rule sets)
   - Closed for modification (core services don't change)

3. **Liskov Substitution Principle (LSP)**
   - Any implementation of `IPricingStrategy` can replace another
   - Any implementation of `IParkingLayoutProvider` can replace another

4. **Interface Segregation Principle (ISP)**
   - Interfaces are focused and specific
   - Clients only depend on methods they use

5. **Dependency Inversion Principle (DIP)**
   - High-level modules depend on abstractions (interfaces)
   - Low-level modules implement those abstractions

### Why Interfaces vs Abstract Classes?

**Interfaces are used** because:
- Multiple implementations may share no common behavior
- We need contracts, not shared implementation
- C# supports multiple interface inheritance
- Easier to mock for testing

**Abstract classes would be used** if:
- Multiple implementations share common behavior
- We need to provide base implementation
- Template method pattern is needed

## 🔧 Extension Points

### Adding a New Institution Type

1. Add the type to `InstitutionType` enum in `Models/Enums.cs`
2. Create configuration in `parkingConfig.json` with the new type
3. Optionally create a custom `IPricingStrategy` implementation for institution-specific pricing

### Adding a New Vehicle Type

1. Add the type to `VehicleType` enum in `Models/Enums.cs`
2. Update configuration to include the new type in `allowedVehicleTypes`
3. No code changes needed - system automatically supports it

### Adding a New Spot Type

1. Add the type to `SpotType` enum in `Models/Enums.cs`
2. Update `StandardPricingStrategy.GetSpotTypeMultiplier()` to handle the new type
3. Add spots with the new type in configuration

### Creating a Custom Pricing Strategy

```csharp
public class PeakHoursPricingStrategy : IPricingStrategy
{
    public decimal CalculateCharge(ParkingTicket ticket, Institution institution, SpotType spotType)
    {
        // Custom pricing logic for peak hours
        // ...
    }
}
```

Then inject it into `PricingService` instead of `StandardPricingStrategy`.

### Replacing JSON Provider with Database

1. Create a new class implementing `IParkingLayoutProvider`
2. Implement `LoadParkingLotAsync()` to read from database
3. Implement `SaveParkingLotAsync()` to write to database
4. Update `Program.cs` to use the new provider

No other code changes needed!

## 📚 API Reference

### IParkingManager

Main interface for parking operations.

#### Methods

```csharp
Task<ParkingTicket?> EnterParkingAsync(VehicleInfo vehicle, SpotType? preferredSpotType = null)
```
Processes vehicle entry, finds spot, assigns it, and generates ticket.

**Parameters:**
- `vehicle`: Vehicle information (license plate, type)
- `preferredSpotType`: Optional preferred spot type (VIP, Accessible, etc.)

**Returns:** Parking ticket with assigned spot, or null if no spot available

---

```csharp
Task<ParkingExitResult?> ExitParkingAsync(string ticketId)
```
Processes vehicle exit, calculates charges, and frees spot.

**Parameters:**
- `ticketId`: Parking ticket identifier

**Returns:** Exit result with charge and duration, or null if ticket not found

---

```csharp
Task<ParkingAvailability> GetAvailabilityAsync()
```
Gets current parking availability information.

**Returns:** Availability status with capacity, occupied, and available counts

### IParkingLayoutProvider

Interface for loading parking lot structure.

#### Methods

```csharp
Task<ParkingLot> LoadParkingLotAsync()
```
Loads parking lot structure from configuration source.

**Returns:** Fully populated ParkingLot instance

---

```csharp
Task SaveParkingLotAsync(ParkingLot parkingLot)
```
Saves parking lot state back to configuration source.

### IPricingStrategy

Interface for calculating parking charges.

#### Methods

```csharp
decimal CalculateCharge(ParkingTicket ticket, Institution institution, SpotType spotType)
```
Calculates total charge for a parking session.

**Parameters:**
- `ticket`: Parking ticket with entry/exit times
- `institution`: Institution configuration with pricing rules
- `spotType`: Type of spot used (affects multipliers)

**Returns:** Total amount to be charged

### IParkingRuleSet

Interface for validating parking rules.

#### Methods

```csharp
bool IsVehicleTypeAllowed(VehicleType vehicleType, Institution institution)
```
Validates if vehicle type is allowed at institution.

---

```csharp
bool CanVehicleParkInSpot(VehicleType vehicleType, Spot spot)
```
Validates if vehicle can park in specific spot.

---

```csharp
bool IsParkingDurationValid(DateTime entryTime, DateTime exitTime, Institution institution)
```
Validates if parking duration is within allowed limits.

## 🧪 Testing the System

### Example Workflow

1. **Start the application:**
   ```bash
   dotnet run
   ```

2. **Check initial availability:**
   - Select option 3
   - Verify total capacity and available spots

3. **Enter a vehicle:**
   - Select option 1
   - Enter license plate: "ABC123"
   - Select vehicle type: Car
   - Optionally select VIP spot
   - Note the ticket ID and spot location

4. **View active tickets:**
   - Select option 4
   - Verify your ticket is listed

5. **Exit the vehicle:**
   - Select option 2
   - Enter ticket ID
   - View parking duration and total charge

6. **Check availability again:**
   - Select option 3
   - Verify spot count increased

## 🔄 Future Enhancements

Potential extensions (not yet implemented):

- [ ] Database integration (replace JSON provider)
- [ ] REST API layer
- [ ] Web UI
- [ ] Real-time availability updates
- [ ] Payment processing integration
- [ ] Reservation system
- [ ] Multi-currency support
- [ ] Advanced pricing (peak hours, discounts, loyalty programs)
- [ ] Reporting and analytics
- [ ] Mobile app integration

## 📝 Notes

- **Configuration is the database**: JSON files simulate a database and can be easily replaced
- **No hard-coded values**: All business rules come from configuration
- **Production-ready structure**: Code follows enterprise patterns and best practices
- **Learning-focused**: Extensive comments explain WHY design decisions were made

## 🤝 Contributing

This is a learning project. Feel free to:
- Add new features
- Improve documentation
- Refactor code
- Add unit tests
- Extend configuration options

## 📄 License

This project is for educational purposes.

---

**Built with ❤️ using C# and .NET**

