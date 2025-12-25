namespace ParkedIt.Models;

/// <summary>
/// Represents a floor in the parking structure containing multiple sections.
/// WHY: Enables multi-level parking structures and floor-level management (enable/disable entire floors).
/// </summary>
public class Floor
{
    /// <summary>
    /// Unique identifier for this floor.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Floor number or name (e.g., "Ground Floor", "Level 1", "B1").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// List of all sections on this floor.
    /// </summary>
    public List<Section> Sections { get; set; } = new();

    /// <summary>
    /// Whether this floor is currently enabled.
    /// Disabled floors are excluded from availability searches.
    /// </summary>
    public bool IsEnabled { get; set; } = true;
}

