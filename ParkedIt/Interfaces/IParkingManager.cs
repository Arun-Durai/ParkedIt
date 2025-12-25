using ParkedIt.Models;

namespace ParkedIt.Interfaces;

/// <summary>
/// Interface for managing parking operations (entry and exit).
/// WHY: Separates parking management logic from data access, enabling testability and multiple implementations.
/// Uses interface because parking management is a contract, not a base class with shared implementation.
/// </summary>
public interface IParkingManager
{
    /// <summary>
    /// Processes vehicle entry: finds available spot, assigns it, and generates a ticket.
    /// </summary>
    /// <param name="vehicle">Information about the entering vehicle.</param>
    /// <param name="preferredSpotType">Optional preferred spot type (VIP, Accessible, etc.).</param>
    /// <returns>Parking ticket with assigned spot location, or null if no spot available.</returns>
    Task<ParkingTicket?> EnterParkingAsync(VehicleInfo vehicle, SpotType? preferredSpotType = null);

    /// <summary>
    /// Processes vehicle exit: validates ticket, calculates charges, and frees the spot.
    /// </summary>
    /// <param name="ticketId">The parking ticket identifier.</param>
    /// <returns>Exit result containing final charge and updated ticket, or null if ticket not found.</returns>
    Task<ParkingExitResult?> ExitParkingAsync(string ticketId);

    /// <summary>
    /// Gets current parking availability information.
    /// </summary>
    /// <returns>Availability status with total capacity, occupied, and available counts.</returns>
    Task<ParkingAvailability> GetAvailabilityAsync();
}

/// <summary>
/// Result of a parking exit operation.
/// WHY: Encapsulates exit result data for better return value handling.
/// </summary>
public class ParkingExitResult
{
    public ParkingTicket Ticket { get; set; } = new();
    public decimal TotalCharge { get; set; }
    public TimeSpan ParkingDuration { get; set; }
}

/// <summary>
/// Current parking availability information.
/// WHY: Provides a clean data structure for availability queries.
/// </summary>
public class ParkingAvailability
{
    public int TotalCapacity { get; set; }
    public int Occupied { get; set; }
    public int Available { get; set; }
    public Dictionary<SpotType, int> AvailableByType { get; set; } = new();
}

