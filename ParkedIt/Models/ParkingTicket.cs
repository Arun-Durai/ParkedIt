namespace ParkedIt.Models;

/// <summary>
/// Represents a parking ticket issued when a vehicle enters the parking lot.
/// WHY: Tracks parking session data (entry time, vehicle, spot) needed for exit and billing.
/// </summary>
public class ParkingTicket
{
    /// <summary>
    /// Unique ticket identifier.
    /// </summary>
    public string TicketId { get; set; } = string.Empty;

    /// <summary>
    /// Information about the parked vehicle.
    /// </summary>
    public VehicleInfo Vehicle { get; set; } = new();

    /// <summary>
    /// Entry timestamp when the vehicle entered.
    /// </summary>
    public DateTime EntryTime { get; set; }

    /// <summary>
    /// Exit timestamp when the vehicle left (null if still parked).
    /// </summary>
    public DateTime? ExitTime { get; set; }

    /// <summary>
    /// Reference to the spot where the vehicle is parked.
    /// Contains floor, section, and spot identifiers.
    /// </summary>
    public SpotLocation? SpotLocation { get; set; }

    /// <summary>
    /// Total amount charged for this parking session.
    /// Calculated on exit.
    /// </summary>
    public decimal TotalCharge { get; set; }

    /// <summary>
    /// Whether the ticket has been paid.
    /// </summary>
    public bool IsPaid { get; set; }
}

/// <summary>
/// Represents the location of a parking spot in the hierarchical structure.
/// WHY: Provides a lightweight reference to spot location without maintaining object references.
/// </summary>
public class SpotLocation
{
    /// <summary>
    /// Floor identifier.
    /// </summary>
    public string FloorId { get; set; } = string.Empty;

    /// <summary>
    /// Section identifier.
    /// </summary>
    public string SectionId { get; set; } = string.Empty;

    /// <summary>
    /// Spot identifier.
    /// </summary>
    public string SpotId { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable location string (e.g., "Floor 1, Section A, Spot 5").
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
}

