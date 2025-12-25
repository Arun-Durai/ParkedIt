using System.Text.Json.Serialization;

namespace ParkedIt.Utils;

/// <summary>
/// JSON configuration model for deserializing parking configuration from JSON files.
/// WHY: Separate DTOs for JSON binding allow clean separation between data transfer and domain models.
/// This enables JSON structure to differ from domain model structure if needed.
/// </summary>
public class ParkingConfigJson
{
    [JsonPropertyName("institution")]
    public InstitutionJson Institution { get; set; } = new();

    [JsonPropertyName("parkingLot")]
    public ParkingLotJson ParkingLot { get; set; } = new();
}

/// <summary>
/// JSON model for institution configuration.
/// </summary>
public class InstitutionJson
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("isFreeParking")]
    public bool IsFreeParking { get; set; }

    [JsonPropertyName("freeMinutes")]
    public int FreeMinutes { get; set; }

    [JsonPropertyName("baseRate")]
    public decimal BaseRate { get; set; }

    [JsonPropertyName("hourlyRate")]
    public decimal HourlyRate { get; set; }

    [JsonPropertyName("rules")]
    public ParkingRulesJson Rules { get; set; } = new();
}

/// <summary>
/// JSON model for parking rules.
/// </summary>
public class ParkingRulesJson
{
    [JsonPropertyName("maxParkingHours")]
    public int MaxParkingHours { get; set; }

    [JsonPropertyName("hasVipSpots")]
    public bool HasVipSpots { get; set; }

    [JsonPropertyName("hasStaffSpots")]
    public bool HasStaffSpots { get; set; }

    [JsonPropertyName("hasAccessibleSpots")]
    public bool HasAccessibleSpots { get; set; }

    [JsonPropertyName("allowedVehicleTypes")]
    public List<string> AllowedVehicleTypes { get; set; } = new();

    [JsonPropertyName("timeBasedRules")]
    public Dictionary<string, string> TimeBasedRules { get; set; } = new();
}

/// <summary>
/// JSON model for parking lot structure.
/// </summary>
public class ParkingLotJson
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("floors")]
    public List<FloorJson> Floors { get; set; } = new();
}

/// <summary>
/// JSON model for floor structure.
/// </summary>
public class FloorJson
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("isEnabled")]
    public bool IsEnabled { get; set; } = true;

    [JsonPropertyName("sections")]
    public List<SectionJson> Sections { get; set; } = new();
}

/// <summary>
/// JSON model for section structure.
/// </summary>
public class SectionJson
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("isEnabled")]
    public bool IsEnabled { get; set; } = true;

    [JsonPropertyName("spots")]
    public List<SpotJson> Spots { get; set; } = new();
}

/// <summary>
/// JSON model for spot structure.
/// </summary>
public class SpotJson
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = "Standard";

    [JsonPropertyName("status")]
    public string Status { get; set; } = "Available";

    [JsonPropertyName("isEnabled")]
    public bool IsEnabled { get; set; } = true;

    [JsonPropertyName("allowedVehicleTypes")]
    public List<string> AllowedVehicleTypes { get; set; } = new();
}

