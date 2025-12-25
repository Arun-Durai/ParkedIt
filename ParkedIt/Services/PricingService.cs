using ParkedIt.Interfaces;
using ParkedIt.Models;

namespace ParkedIt.Services;

/// <summary>
/// Service for calculating parking charges using institution-specific pricing strategies.
/// WHY: Centralizes pricing logic and delegates to strategy pattern for institution-specific calculations.
/// Follows Open/Closed Principle - open for extension (new strategies) but closed for modification.
/// </summary>
public class PricingService
{
    private readonly IPricingStrategy _pricingStrategy;

    /// <summary>
    /// Constructor with dependency injection of pricing strategy.
    /// WHY: Strategy pattern - allows different pricing algorithms per institution without changing this service.
    /// </summary>
    public PricingService(IPricingStrategy pricingStrategy)
    {
        _pricingStrategy = pricingStrategy ?? throw new ArgumentNullException(nameof(pricingStrategy));
    }

    /// <summary>
    /// Calculates the total charge for a parking session.
    /// WHY: Delegates to strategy pattern, allowing institution-specific pricing rules.
    /// </summary>
    /// <param name="ticket">Parking ticket with entry/exit times.</param>
    /// <param name="institution">Institution configuration with pricing rules.</param>
    /// <param name="spotType">Type of spot used (may affect pricing multipliers).</param>
    /// <returns>Total amount to be charged.</returns>
    public decimal CalculateCharge(ParkingTicket ticket, Institution institution, SpotType spotType)
    {
        if (ticket.ExitTime == null)
        {
            throw new InvalidOperationException("Cannot calculate charge for ticket without exit time.");
        }

        // If parking is free, return zero
        if (institution.IsFreeParking)
        {
            return 0;
        }

        // Delegate to strategy for calculation
        return _pricingStrategy.CalculateCharge(ticket, institution, spotType);
    }
}

