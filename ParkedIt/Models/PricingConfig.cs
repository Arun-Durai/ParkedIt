namespace ParkedIt.Models;

/// <summary>
/// Configuration model for pricing calculations.
/// WHY: Separates pricing configuration from business logic, enabling easy updates via JSON.
/// </summary>
public class PricingConfig
{
    /// <summary>
    /// Base parking fee charged on entry (one-time).
    /// </summary>
    public decimal BaseRate { get; set; }

    /// <summary>
    /// Hourly rate charged after free minutes expire.
    /// </summary>
    public decimal HourlyRate { get; set; }

    /// <summary>
    /// Number of free minutes before billing starts.
    /// </summary>
    public int FreeMinutes { get; set; }

    /// <summary>
    /// Whether parking is completely free (no charges).
    /// </summary>
    public bool IsFreeParking { get; set; }

    /// <summary>
    /// Optional multipliers for different spot types.
    /// Key: SpotType, Value: Multiplier (e.g., VIP = 1.5x)
    /// </summary>
    public Dictionary<string, decimal> SpotTypeMultipliers { get; set; } = new();

    /// <summary>
    /// Optional multipliers for different vehicle types.
    /// Key: VehicleType, Value: Multiplier
    /// </summary>
    public Dictionary<string, decimal> VehicleTypeMultipliers { get; set; } = new();
}

