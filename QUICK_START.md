# Quick Start Guide

## 🚀 Running the Application

```bash
cd /Users/arun/Projects/ParkedIt/ParkedIt
dotnet run
```

## 📝 Common Tasks

### Modify Parking Structure

Edit `Config/parkingConfig.json`:
- Add/remove floors
- Add/remove sections
- Add/remove spots
- Enable/disable floors, sections, or spots

### Change Pricing

Edit `Config/parkingConfig.json` → `institution` section:
```json
{
  "institution": {
    "baseRate": 10.00,      // Entry fee
    "hourlyRate": 5.00,     // Hourly charge
    "freeMinutes": 30       // Free parking time
  }
}
```

### Add New Vehicle Type

1. Add to `Models/Enums.cs`:
```csharp
public enum VehicleType
{
    Car,
    Bike,
    EV,
    Truck,
    Auto,
    Bicycle,
    Motorcycle  // NEW
}
```

2. Update JSON config:
```json
{
  "allowedVehicleTypes": ["Car", "Bike", "Motorcycle"]
}
```

### Add New Spot Type

1. Add to `Models/Enums.cs`:
```csharp
public enum SpotType
{
    Standard,
    VIP,
    Staff,
    Accessible,
    Electric  // NEW (for EV charging)
}
```

2. Update pricing in `Services/StandardPricingStrategy.cs`:
```csharp
private decimal GetSpotTypeMultiplier(SpotType spotType)
{
    return spotType switch
    {
        SpotType.Electric => 1.2m,  // NEW
        // ... rest
    };
}
```

3. Add spots in JSON config:
```json
{
  "spots": [
    {
      "id": "SPOT-021",
      "type": "Electric",
      "status": "Available"
    }
  ]
}
```

### Create Custom Pricing Strategy

1. Create new class:
```csharp
public class PeakHoursPricingStrategy : IPricingStrategy
{
    public decimal CalculateCharge(ParkingTicket ticket, Institution institution, SpotType spotType)
    {
        var hour = ticket.EntryTime.Hour;
        var isPeak = hour >= 9 && hour <= 17;
        var multiplier = isPeak ? 1.5m : 1.0m;
        
        // Use standard calculation with multiplier
        // ...
    }
}
```

2. Update `Program.cs`:
```csharp
var pricingStrategy = new PeakHoursPricingStrategy();  // Changed
var pricingService = new PricingService(pricingStrategy);
```

## 🔍 Finding Code

| What you need | Where to look |
|---------------|---------------|
| Parking structure models | `Models/ParkingLot.cs`, `Models/Floor.cs`, etc. |
| Business logic | `Services/ParkingManagerService.cs` |
| Spot finding logic | `Services/LayoutService.cs` |
| Pricing calculation | `Services/StandardPricingStrategy.cs` |
| Rule validation | `Services/InstitutionRuleEngine.cs` |
| JSON loading | `Services/JsonParkingLayoutProvider.cs` |
| Configuration format | `Config/parkingConfig.json` |
| Entry point | `Program.cs` |

## 🐛 Debugging Tips

### Configuration Not Loading

1. Check file path in `Program.cs` → `InitializeServices()`
2. Verify JSON is valid (use JSON validator)
3. Check file permissions

### Spot Not Found

1. Check spot `isEnabled` flag in config
2. Check spot `status` (must be "Available")
3. Check vehicle type restrictions
4. Check floor/section `isEnabled` flags

### Pricing Calculation Wrong

1. Check `institution.IsFreeParking` flag
2. Verify `freeMinutes`, `baseRate`, `hourlyRate` in config
3. Check spot type multiplier in `StandardPricingStrategy`
4. Verify entry/exit times in ticket

## 📚 Key Concepts

### Configuration-Driven
- All business rules come from JSON
- No hard-coded values
- Change behavior without recompiling

### Dependency Injection
- Services receive dependencies via constructor
- Dependencies are interfaces (abstractions)
- Easy to swap implementations

### Strategy Pattern
- Pricing strategies are swappable
- Add new strategies without changing existing code
- Follows Open/Closed Principle

## 🔄 Common Workflows

### Testing Entry Flow
1. Run application
2. Select option 1 (Vehicle Entry)
3. Enter license plate
4. Select vehicle type
5. Verify ticket generated
6. Check spot assigned

### Testing Exit Flow
1. Enter a vehicle first
2. Note the ticket ID
3. Select option 2 (Vehicle Exit)
4. Enter ticket ID
5. Verify charge calculated
6. Check spot freed

### Testing Availability
1. Select option 3 (Check Availability)
2. Note total capacity
3. Enter a vehicle
4. Check availability again
5. Verify count decreased

## 💡 Pro Tips

1. **Use JSON validator** to catch config errors early
2. **Check console output** for detailed error messages
3. **Start with simple config** then add complexity
4. **Test one feature at a time** (entry, then exit, then availability)
5. **Read code comments** - they explain WHY, not just WHAT

## 🆘 Troubleshooting

| Problem | Solution |
|---------|----------|
| "Configuration file not found" | Check file path, verify file exists |
| "No available spots" | Check all spots are enabled and available |
| "Vehicle type not allowed" | Update `allowedVehicleTypes` in config |
| "Invalid institution type" | Check enum value matches JSON |
| Build errors | Run `dotnet clean` then `dotnet build` |

---

**Need more help?** See [README.md](README.md) for detailed documentation or [ARCHITECTURE.md](ARCHITECTURE.md) for design details.

