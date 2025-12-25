namespace ParkedIt.Models;

/// <summary>
/// Represents a section within a floor containing multiple parking spots.
/// WHY: Organizes spots into logical groups (e.g., "North Wing", "Level A") for better management.
/// </summary>
public class Section
{
    /// <summary>
    /// Unique identifier for this section within its floor.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the section (e.g., "North Wing", "Level A").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// List of all parking spots in this section.
    /// </summary>
    public List<Spot> Spots { get; set; } = new();

    /// <summary>
    /// Whether this section is currently enabled.
    /// Disabled sections are excluded from availability searches.
    /// </summary>
    public bool IsEnabled { get; set; } = true;
}

