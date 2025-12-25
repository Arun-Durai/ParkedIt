using ParkedIt.Models;

namespace ParkedIt.Interfaces;

/// <summary>
/// Interface for validating parking rules and constraints.
/// WHY: Separates rule validation from business logic, enabling extensible rule engines.
/// Uses interface because rule sets are contracts that can be implemented differently per institution.
/// </summary>
public interface IParkingRuleSet
{
    /// <summary>
    /// Validates if a vehicle type is allowed to park at this institution.
    /// </summary>
    /// <param name="vehicleType">Type of vehicle attempting to park.</param>
    /// <param name="institution">Institution configuration with rules.</param>
    /// <returns>True if vehicle type is allowed, false otherwise.</returns>
    bool IsVehicleTypeAllowed(VehicleType vehicleType, Institution institution);

    /// <summary>
    /// Validates if a vehicle can park in a specific spot.
    /// </summary>
    /// <param name="vehicleType">Type of vehicle.</param>
    /// <param name="spot">The parking spot to validate.</param>
    /// <returns>True if vehicle can park in this spot, false otherwise.</returns>
    bool CanVehicleParkInSpot(VehicleType vehicleType, Spot spot);

    /// <summary>
    /// Validates if parking duration is within allowed limits.
    /// </summary>
    /// <param name="entryTime">Entry timestamp.</param>
    /// <param name="exitTime">Exit timestamp.</param>
    /// <param name="institution">Institution configuration with rules.</param>
    /// <returns>True if duration is valid, false if exceeds maximum allowed time.</returns>
    bool IsParkingDurationValid(DateTime entryTime, DateTime exitTime, Institution institution);
}

