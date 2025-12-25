using ParkedIt.Interfaces;
using ParkedIt.Models;

namespace ParkedIt.Services;

/// <summary>
/// Service for managing parking lot layout and structure.
/// WHY: Centralizes layout operations (finding spots, updating availability) in one place.
/// Follows Single Responsibility Principle - only handles layout concerns.
/// </summary>
public class LayoutService
{
    private readonly IParkingLayoutProvider _layoutProvider;
    private ParkingLot? _parkingLot;

    /// <summary>
    /// Constructor with dependency injection of layout provider.
    /// WHY: Dependency inversion - depends on abstraction (interface) not concrete implementation.
    /// </summary>
    public LayoutService(IParkingLayoutProvider layoutProvider)
    {
        _layoutProvider = layoutProvider ?? throw new ArgumentNullException(nameof(layoutProvider));
    }

    /// <summary>
    /// Loads the parking lot structure from the configuration provider.
    /// WHY: Lazy loading pattern - only loads when needed, can be refreshed if config changes.
    /// </summary>
    public async Task<ParkingLot> GetParkingLotAsync()
    {
        if (_parkingLot == null)
        {
            _parkingLot = await _layoutProvider.LoadParkingLotAsync();
        }
        return _parkingLot;
    }

    /// <summary>
    /// Finds an available spot matching the criteria.
    /// WHY: Encapsulates complex spot-finding logic with filtering by vehicle type, spot type, and availability.
    /// </summary>
    /// <param name="vehicleType">Type of vehicle looking for a spot.</param>
    /// <param name="preferredSpotType">Optional preferred spot type (VIP, Accessible, etc.).</param>
    /// <param name="ruleSet">Rule set to validate spot eligibility.</param>
    /// <param name="institution">Institution configuration for rule validation.</param>
    /// <returns>Tuple of (Floor, Section, Spot) if found, null otherwise.</returns>
    public async Task<(Floor Floor, Section Section, Spot Spot)?> FindAvailableSpotAsync(
        VehicleType vehicleType,
        SpotType? preferredSpotType,
        IParkingRuleSet ruleSet,
        Institution institution)
    {
        var parkingLot = await GetParkingLotAsync();

        // First, try to find preferred spot type if specified
        if (preferredSpotType.HasValue)
        {
            var preferredSpot = FindSpotByType(parkingLot, vehicleType, preferredSpotType.Value, ruleSet, institution);
            if (preferredSpot.HasValue)
                return preferredSpot;
        }

        // Fall back to any available spot
        return FindSpotByType(parkingLot, vehicleType, SpotType.Standard, ruleSet, institution) ??
               FindAnyAvailableSpot(parkingLot, vehicleType, ruleSet, institution);
    }

    /// <summary>
    /// Finds a spot of a specific type.
    /// WHY: Helper method to avoid code duplication in spot-finding logic.
    /// </summary>
    private (Floor Floor, Section Section, Spot Spot)? FindSpotByType(
        ParkingLot parkingLot,
        VehicleType vehicleType,
        SpotType spotType,
        IParkingRuleSet ruleSet,
        Institution institution)
    {
        foreach (var floor in parkingLot.Floors.Where(f => f.IsEnabled))
        {
            foreach (var section in floor.Sections.Where(s => s.IsEnabled))
            {
                foreach (var spot in section.Spots.Where(s => s.IsEnabled))
                {
                    if (spot.Type == spotType &&
                        spot.Status == AvailabilityStatus.Available &&
                        ruleSet.CanVehicleParkInSpot(vehicleType, spot))
                    {
                        return (floor, section, spot);
                    }
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Finds any available spot that matches vehicle type requirements.
    /// WHY: Fallback when preferred spot type is not available.
    /// </summary>
    private (Floor Floor, Section Section, Spot Spot)? FindAnyAvailableSpot(
        ParkingLot parkingLot,
        VehicleType vehicleType,
        IParkingRuleSet ruleSet,
        Institution institution)
    {
        foreach (var floor in parkingLot.Floors.Where(f => f.IsEnabled))
        {
            foreach (var section in floor.Sections.Where(s => s.IsEnabled))
            {
                foreach (var spot in section.Spots.Where(s => s.IsEnabled))
                {
                    if (spot.Status == AvailabilityStatus.Available &&
                        ruleSet.CanVehicleParkInSpot(vehicleType, spot))
                    {
                        return (floor, section, spot);
                    }
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Assigns a vehicle to a spot and updates the spot status.
    /// WHY: Encapsulates spot assignment logic to ensure consistent state updates.
    /// </summary>
    public void AssignSpotToVehicle(Spot spot, ParkingTicket ticket)
    {
        spot.Status = AvailabilityStatus.Occupied;
        spot.CurrentTicket = ticket;
    }

    /// <summary>
    /// Frees a spot when a vehicle exits.
    /// WHY: Encapsulates spot release logic to ensure consistent state cleanup.
    /// </summary>
    public void FreeSpot(Spot spot)
    {
        spot.Status = AvailabilityStatus.Available;
        spot.CurrentTicket = null;
    }

    /// <summary>
    /// Gets availability statistics for the parking lot.
    /// WHY: Provides aggregated availability data without exposing internal structure.
    /// </summary>
    public async Task<ParkingAvailability> GetAvailabilityAsync()
    {
        var parkingLot = await GetParkingLotAsync();
        var availability = new ParkingAvailability
        {
            TotalCapacity = parkingLot.TotalCapacity,
            Occupied = parkingLot.OccupiedCount,
            Available = parkingLot.AvailableCount
        };

        // Count available spots by type
        foreach (var floor in parkingLot.Floors.Where(f => f.IsEnabled))
        {
            foreach (var section in floor.Sections.Where(s => s.IsEnabled))
            {
                foreach (var spot in section.Spots.Where(s => s.IsEnabled && s.Status == AvailabilityStatus.Available))
                {
                    if (!availability.AvailableByType.ContainsKey(spot.Type))
                        availability.AvailableByType[spot.Type] = 0;
                    availability.AvailableByType[spot.Type]++;
                }
            }
        }

        return availability;
    }

    /// <summary>
    /// Locates a spot by its hierarchical identifiers.
    /// WHY: Enables finding spots by ticket location data without maintaining object references.
    /// </summary>
    public async Task<(Floor Floor, Section Section, Spot Spot)?> LocateSpotAsync(SpotLocation location)
    {
        var parkingLot = await GetParkingLotAsync();

        var floor = parkingLot.Floors.FirstOrDefault(f => f.Id == location.FloorId);
        if (floor == null) return null;

        var section = floor.Sections.FirstOrDefault(s => s.Id == location.SectionId);
        if (section == null) return null;

        var spot = section.Spots.FirstOrDefault(s => s.Id == location.SpotId);
        if (spot == null) return null;

        return (floor, section, spot);
    }
}

