using SalesService.Domain.Events;

namespace SalesService.Application.Interfaces;

/// <summary>
/// Interface for publishing domain events
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publishes a domain event
    /// </summary>
    /// <param name="event">The domain event to publish</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task PublishAsync(ISaleEvent @event, CancellationToken cancellationToken = default);
} 