using ParkedIt.Interfaces;
using ParkedIt.Models;

namespace ParkedIt.Services;

/// <summary>
/// Standard pricing strategy implementation.
/// WHY: Implements IPricingStrategy for standard pricing calculation (base rate + hourly rate after free minutes).
/// Uses interface implementation because this is one of potentially many pricing strategies.
/// </summary>
public class StandardPricingStrategy : IPricingStrategy
{
    /// <summary>
    /// Calculates parking charge using standard formula:
    /// Base Rate + (Hours after free minutes * Hourly Rate) * Spot/Vehicle multipliers
    /// WHY: Encapsulates standard pricing logic that can be replaced with other strategies (peak hours, discounts, etc.).
    /// </summary>
    public decimal CalculateCharge(ParkingTicket ticket, Institution institution, SpotType spotType)
    {
        if (ticket.ExitTime == null)
        {
            throw new ArgumentException("Exit time is required for charge calculation.", nameof(ticket));
        }

        var duration = ticket.ExitTime.Value - ticket.EntryTime;
        var totalMinutes = (int)duration.TotalMinutes;

        // Start with base rate
        decimal totalCharge = institution.BaseRate;

        // Calculate billable minutes (after free minutes)
        var billableMinutes = Math.Max(0, totalMinutes - institution.FreeMinutes);

        if (billableMinutes > 0)
        {
            // Calculate billable hours (round up)
            var billableHours = (int)Math.Ceiling(billableMinutes / 60.0);
            totalCharge += billableHours * institution.HourlyRate;
        }

        // Apply spot type multiplier if configured
        // Note: In a full implementation, multipliers would come from institution config
        // For now, we'll use a simple multiplier based on spot type
        var spotMultiplier = GetSpotTypeMultiplier(spotType);
        totalCharge *= spotMultiplier;

        return totalCharge;
    }

    /// <summary>
    /// Gets multiplier for spot type.
    /// WHY: Allows VIP spots to charge more, accessible spots to charge less, etc.
    /// In production, this would come from institution configuration.
    /// </summary>
    private decimal GetSpotTypeMultiplier(SpotType spotType)
    {
        return spotType switch
        {
            SpotType.VIP => 1.5m,           // VIP spots cost 50% more
            SpotType.Accessible => 0.8m,    // Accessible spots cost 20% less
            SpotType.Staff => 0m,           // Staff spots are free
            _ => 1.0m                        // Standard spots have no multiplier
        };
    }
}

