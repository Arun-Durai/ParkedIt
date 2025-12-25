using ParkedIt.Models;

namespace ParkedIt.Interfaces;

/// <summary>
/// Interface for loading and providing parking lot layout data.
/// WHY: Interface allows swapping JSON file provider with database provider later without changing dependent code.
/// Uses interface (not abstract class) because multiple implementations may share no common behavior.
/// </summary>
public interface IParkingLayoutProvider
{
    /// <summary>
    /// Loads the parking lot structure from configuration source.
    /// </summary>
    /// <returns>Fully populated ParkingLot instance with all floors, sections, and spots.</returns>
    Task<ParkingLot> LoadParkingLotAsync();

    /// <summary>
    /// Saves the current parking lot state back to the configuration source.
    /// Useful for persisting spot availability changes.
    /// </summary>
    /// <param name="parkingLot">The parking lot to save.</param>
    Task SaveParkingLotAsync(ParkingLot parkingLot);
}

