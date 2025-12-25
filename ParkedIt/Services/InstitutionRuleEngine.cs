using ParkedIt.Interfaces;
using ParkedIt.Models;

namespace ParkedIt.Services;

/// <summary>
/// Engine for validating parking rules based on institution configuration.
/// WHY: Implements IParkingRuleSet interface, centralizing all rule validation logic.
/// Uses interface implementation (not abstract class) because rule validation is a contract, not shared behavior.
/// </summary>
public class InstitutionRuleEngine : IParkingRuleSet
{
    /// <summary>
    /// Validates if a vehicle type is allowed to park at this institution.
    /// WHY: Enforces institution-level vehicle restrictions (e.g., "No trucks at mall").
    /// </summary>
    public bool IsVehicleTypeAllowed(VehicleType vehicleType, Institution institution)
    {
        // If no restrictions specified, all vehicle types are allowed
        if (institution.Rules.AllowedVehicleTypes == null || institution.Rules.AllowedVehicleTypes.Count == 0)
        {
            return true;
        }

        return institution.Rules.AllowedVehicleTypes.Contains(vehicleType);
    }

    /// <summary>
    /// Validates if a vehicle can park in a specific spot.
    /// WHY: Enforces spot-level restrictions (vehicle type compatibility, spot type access).
    /// </summary>
    public bool CanVehicleParkInSpot(VehicleType vehicleType, Spot spot)
    {
        // Disabled spots cannot be used
        if (!spot.IsEnabled || spot.Status != AvailabilityStatus.Available)
        {
            return false;
        }

        // If spot has no vehicle type restrictions, all types are allowed
        if (spot.AllowedVehicleTypes == null || spot.AllowedVehicleTypes.Count == 0)
        {
            return true;
        }

        // Check if vehicle type is in the allowed list
        return spot.AllowedVehicleTypes.Contains(vehicleType);
    }

    /// <summary>
    /// Validates if parking duration is within allowed limits.
    /// WHY: Enforces maximum parking duration rules (e.g., "Max 4 hours at mall").
    /// </summary>
    public bool IsParkingDurationValid(DateTime entryTime, DateTime exitTime, Institution institution)
    {
        // If no maximum duration specified, any duration is valid
        if (institution.Rules.MaxParkingHours <= 0)
        {
            return true;
        }

        var duration = exitTime - entryTime;
        var maxDuration = TimeSpan.FromHours(institution.Rules.MaxParkingHours);

        return duration <= maxDuration;
    }
}

