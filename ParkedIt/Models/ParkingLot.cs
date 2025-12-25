namespace ParkedIt.Models;

/// <summary>
/// Root model representing the entire parking lot structure.
/// WHY: Centralizes parking layout data and provides hierarchical access (Lot -> Floor -> Section -> Spot).
/// </summary>
public class ParkingLot
{
    /// <summary>
    /// Unique identifier for the parking lot.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the parking lot.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Reference to the institution that owns/manages this parking lot.
    /// </summary>
    public Institution Institution { get; set; } = new();

    /// <summary>
    /// List of all floors in the parking structure.
    /// </summary>
    public List<Floor> Floors { get; set; } = new();

    /// <summary>
    /// Total capacity of the parking lot (sum of all enabled spots).
    /// </summary>
    public int TotalCapacity => Floors
        .Where(f => f.IsEnabled)
        .SelectMany(f => f.Sections)
        .Where(s => s.IsEnabled)
        .SelectMany(s => s.Spots)
        .Count(s => s.IsEnabled);

    /// <summary>
    /// Current number of occupied spots.
    /// </summary>
    public int OccupiedCount => Floors
        .SelectMany(f => f.Sections)
        .SelectMany(s => s.Spots)
        .Count(s => s.Status == AvailabilityStatus.Occupied);

    /// <summary>
    /// Current number of available spots.
    /// </summary>
    public int AvailableCount => TotalCapacity - OccupiedCount;
}

