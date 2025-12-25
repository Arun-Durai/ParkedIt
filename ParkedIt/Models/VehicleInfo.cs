namespace ParkedIt.Models;

/// <summary>
/// Represents information about a vehicle entering or exiting the parking lot.
/// WHY: Encapsulates vehicle data separately from parking logic for better separation of concerns.
/// </summary>
public class VehicleInfo
{
    /// <summary>
    /// Vehicle registration/license plate number.
    /// Used as unique identifier for the vehicle.
    /// </summary>
    public string LicensePlate { get; set; } = string.Empty;

    /// <summary>
    /// Type of vehicle (Car, Bike, EV, etc.).
    /// Determines which spots the vehicle can use.
    /// </summary>
    public VehicleType Type { get; set; }

    /// <summary>
    /// Optional vehicle owner/driver name.
    /// </summary>
    public string? OwnerName { get; set; }

    /// <summary>
    /// Optional contact information.
    /// </summary>
    public string? ContactInfo { get; set; }
}

