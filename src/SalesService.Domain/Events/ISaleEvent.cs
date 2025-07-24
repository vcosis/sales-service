namespace SalesService.Domain.Events;

/// <summary>
/// Base interface for all sale-related domain events
/// </summary>
public interface ISaleEvent
{
    /// <summary>
    /// The ID of the sale that triggered the event
    /// </summary>
    int SaleId { get; }
    
    /// <summary>
    /// The timestamp when the event occurred
    /// </summary>
    DateTime OccurredAt { get; }
} 