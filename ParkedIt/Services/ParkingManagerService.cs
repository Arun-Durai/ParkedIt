using ParkedIt.Interfaces;
using ParkedIt.Models;

namespace ParkedIt.Services;

/// <summary>
/// Main service for managing parking operations (entry and exit).
/// WHY: Orchestrates parking flows by coordinating layout, pricing, and rule services.
/// Follows Facade pattern - provides simple interface to complex subsystem.
/// </summary>
public class ParkingManagerService : IParkingManager
{
    private readonly LayoutService _layoutService;
    private readonly PricingService _pricingService;
    private readonly IParkingRuleSet _ruleSet;
    private readonly IParkingLayoutProvider _layoutProvider;
    private readonly Dictionary<string, ParkingTicket> _activeTickets = new();

    /// <summary>
    /// Constructor with dependency injection of required services.
    /// WHY: Dependency inversion - depends on abstractions (interfaces) not concrete classes.
    /// </summary>
    public ParkingManagerService(
        LayoutService layoutService,
        PricingService pricingService,
        IParkingRuleSet ruleSet,
        IParkingLayoutProvider layoutProvider)
    {
        _layoutService = layoutService ?? throw new ArgumentNullException(nameof(layoutService));
        _pricingService = pricingService ?? throw new ArgumentNullException(nameof(pricingService));
        _ruleSet = ruleSet ?? throw new ArgumentNullException(nameof(ruleSet));
        _layoutProvider = layoutProvider ?? throw new ArgumentNullException(nameof(layoutProvider));
    }

    /// <summary>
    /// Processes vehicle entry: validates rules, finds spot, assigns it, and generates ticket.
    /// WHY: Encapsulates complete entry flow, ensuring all validations and state updates happen atomically.
    /// </summary>
    public async Task<ParkingTicket?> EnterParkingAsync(VehicleInfo vehicle, SpotType? preferredSpotType = null)
    {
        // Load parking lot to get institution configuration
        var parkingLot = await _layoutService.GetParkingLotAsync();

        // Validate vehicle type is allowed at this institution
        if (!_ruleSet.IsVehicleTypeAllowed(vehicle.Type, parkingLot.Institution))
        {
            throw new InvalidOperationException($"Vehicle type {vehicle.Type} is not allowed at {parkingLot.Institution.Name}.");
        }

        // Find available spot
        var spotLocation = await _layoutService.FindAvailableSpotAsync(
            vehicle.Type,
            preferredSpotType,
            _ruleSet,
            parkingLot.Institution);

        if (!spotLocation.HasValue)
        {
            return null; // No available spots
        }

        var (floor, section, spot) = spotLocation.Value;

        // Generate ticket
        var ticket = new ParkingTicket
        {
            TicketId = GenerateTicketId(),
            Vehicle = vehicle,
            EntryTime = DateTime.Now,
            SpotLocation = new SpotLocation
            {
                FloorId = floor.Id,
                SectionId = section.Id,
                SpotId = spot.Id,
                DisplayName = $"{floor.Name}, {section.Name}, Spot {spot.Id}"
            }
        };

        // Assign spot to vehicle
        _layoutService.AssignSpotToVehicle(spot, ticket);

        // Store ticket in active tickets dictionary
        _activeTickets[ticket.TicketId] = ticket;

        return ticket;
    }

    /// <summary>
    /// Processes vehicle exit: validates ticket, calculates charge, and frees the spot.
    /// WHY: Encapsulates complete exit flow, ensuring billing and state cleanup happen together.
    /// </summary>
    public async Task<ParkingExitResult?> ExitParkingAsync(string ticketId)
    {
        // Find ticket
        if (!_activeTickets.TryGetValue(ticketId, out var ticket))
        {
            return null; // Ticket not found
        }

        // Set exit time
        ticket.ExitTime = DateTime.Now;

        // Load parking lot for institution configuration
        var parkingLot = await _layoutService.GetParkingLotAsync();

        // Validate parking duration
        if (!_ruleSet.IsParkingDurationValid(ticket.EntryTime, ticket.ExitTime.Value, parkingLot.Institution))
        {
            throw new InvalidOperationException($"Parking duration exceeds maximum allowed time of {parkingLot.Institution.Rules.MaxParkingHours} hours.");
        }

        // Locate the spot
        if (ticket.SpotLocation == null)
        {
            throw new InvalidOperationException("Ticket does not have spot location information.");
        }

        var spotLocation = await _layoutService.LocateSpotAsync(ticket.SpotLocation);
        if (!spotLocation.HasValue)
        {
            throw new InvalidOperationException("Could not locate spot for ticket.");
        }

        var (_, _, spot) = spotLocation.Value;

        // Calculate charge
        var totalCharge = _pricingService.CalculateCharge(ticket, parkingLot.Institution, spot.Type);
        ticket.TotalCharge = totalCharge;

        // Free the spot
        _layoutService.FreeSpot(spot);

        // Remove from active tickets
        _activeTickets.Remove(ticketId);

        // Create exit result
        var result = new ParkingExitResult
        {
            Ticket = ticket,
            TotalCharge = totalCharge,
            ParkingDuration = ticket.ExitTime.Value - ticket.EntryTime
        };

        return result;
    }

    /// <summary>
    /// Gets current parking availability information.
    /// WHY: Delegates to layout service for availability data.
    /// </summary>
    public async Task<ParkingAvailability> GetAvailabilityAsync()
    {
        return await _layoutService.GetAvailabilityAsync();
    }

    /// <summary>
    /// Generates a unique ticket identifier.
    /// WHY: Encapsulates ticket ID generation logic, can be replaced with GUID or database sequence later.
    /// </summary>
    private string GenerateTicketId()
    {
        return $"TKT-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }
}

