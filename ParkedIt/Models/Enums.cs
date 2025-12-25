namespace ParkedIt.Models;

/// <summary>
/// Defines the type of institution where parking is managed.
/// Each institution type may have different pricing rules, free minutes, and slot availability.
/// </summary>
public enum InstitutionType
{
    Mall,
    Theatre,
    Hospital,
    College,
    Corporate
}

/// <summary>
/// Represents different vehicle types that can park in the system.
/// Extensible via configuration - new types can be added without code changes.
/// </summary>
public enum VehicleType
{
    Car,
    Bike,
    EV,      // Electric Vehicle
    Truck,
    Auto,    // Auto-rickshaw
    Bicycle
}

/// <summary>
/// Defines special attributes or categories for parking spots.
/// These determine access rules and may affect pricing or availability.
/// </summary>
public enum SpotType
{
    Standard,    // Regular parking spot
    VIP,         // Premium/VIP parking
    Staff,       // Staff-only parking
    Accessible   // Accessible/disabled parking
}

/// <summary>
/// Current availability status of a parking spot.
/// Used to track real-time state and enable/disable spots dynamically.
/// </summary>
public enum AvailabilityStatus
{
    Available,   // Spot is free and ready for use
    Occupied,    // Currently occupied by a vehicle
    Disabled,    // Temporarily disabled (maintenance, etc.)
    Reserved     // Reserved for specific use
}

