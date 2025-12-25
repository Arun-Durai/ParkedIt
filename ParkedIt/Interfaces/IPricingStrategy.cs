using ParkedIt.Models;

namespace ParkedIt.Interfaces;

/// <summary>
/// Interface for calculating parking charges based on institution-specific rules.
/// WHY: Strategy pattern allows different pricing algorithms per institution without modifying core logic.
/// Uses interface (not abstract class) because pricing strategies may have completely different implementations.
/// </summary>
public interface IPricingStrategy
{
    /// <summary>
    /// Calculates the total charge for a parking session.
    /// </summary>
    /// <param name="ticket">The parking ticket with entry/exit times.</param>
    /// <param name="institution">The institution configuration with pricing rules.</param>
    /// <param name="spotType">Type of spot used (may affect pricing).</param>
    /// <returns>Total amount to be charged.</returns>
    decimal CalculateCharge(ParkingTicket ticket, Institution institution, SpotType spotType);
}

