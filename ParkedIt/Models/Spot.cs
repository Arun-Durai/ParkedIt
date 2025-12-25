namespace ParkedIt.Models;

/// <summary>
/// Represents a single parking spot within a section.
/// WHY: Encapsulates spot-level state and enables individual spot management (enable/disable).
/// </summary>
public class Spot
{
    /// <summary>
    /// Unique identifier for this spot within its section.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Type of spot (Standard, VIP, Staff, Accessible).
    /// Determines access rules and may affect pricing.
    /// </summary>
    public SpotType Type { get; set; }

    /// <summary>
    /// Current availability status of the spot.
    /// </summary>
    public AvailabilityStatus Status { get; set; } = AvailabilityStatus.Available;

    /// <summary>
    /// Vehicle types that can park in this spot.
    /// Empty list means all types are allowed.
    /// </summary>
    public List<VehicleType> AllowedVehicleTypes { get; set; } = new();

    /// <summary>
    /// Reference to the parking ticket if this spot is occupied.
    /// Null when spot is available.
    /// </summary>
    public ParkingTicket? CurrentTicket { get; set; }

    /// <summary>
    /// Whether this spot is currently enabled for use.
    /// Disabled spots are excluded from availability searches.
    /// </summary>
    public bool IsEnabled { get; set; } = true;
}

