namespace ParkedIt.Models;

/// <summary>
/// Represents an institution (Mall, Theatre, etc.) with its specific configuration.
/// This model binds to JSON configuration and contains all institution-specific rules.
/// WHY: Centralizes institution data to enable behavior variation without code changes.
/// </summary>
public class Institution
{
    /// <summary>
    /// Unique identifier for the institution.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the institution.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Type of institution (affects pricing and rules).
    /// </summary>
    public InstitutionType Type { get; set; }

    /// <summary>
    /// Whether parking is free at this institution.
    /// If false, pricing rules apply.
    /// </summary>
    public bool IsFreeParking { get; set; }

    /// <summary>
    /// Number of free minutes before billing starts.
    /// Applies only if IsFreeParking is false.
    /// </summary>
    public int FreeMinutes { get; set; }

    /// <summary>
    /// Base parking fee (one-time charge on entry).
    /// </summary>
    public decimal BaseRate { get; set; }

    /// <summary>
    /// Hourly rate charged after free minutes expire.
    /// </summary>
    public decimal HourlyRate { get; set; }

    /// <summary>
    /// Institution-specific parking rules and constraints.
    /// </summary>
    public ParkingRules Rules { get; set; } = new();
}

