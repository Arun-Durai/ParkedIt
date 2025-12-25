namespace ParkedIt.Models;

/// <summary>
/// Defines institution-specific parking rules and constraints.
/// WHY: Separates rules from core models to allow flexible rule configuration per institution.
/// </summary>
public class ParkingRules
{
    /// <summary>
    /// Maximum parking duration in hours (0 = unlimited).
    /// </summary>
    public int MaxParkingHours { get; set; }

    /// <summary>
    /// Whether VIP spots are available at this institution.
    /// </summary>
    public bool HasVipSpots { get; set; }

    /// <summary>
    /// Whether staff-only spots exist.
    /// </summary>
    public bool HasStaffSpots { get; set; }

    /// <summary>
    /// Whether accessible parking spots are available.
    /// </summary>
    public bool HasAccessibleSpots { get; set; }

    /// <summary>
    /// Allowed vehicle types for this institution.
    /// Empty list means all types are allowed.
    /// </summary>
    public List<VehicleType> AllowedVehicleTypes { get; set; } = new();

    /// <summary>
    /// Time-based rules (e.g., peak hours, special rates).
    /// Extensible for future time-based pricing.
    /// </summary>
    public Dictionary<string, string> TimeBasedRules { get; set; } = new();
}

