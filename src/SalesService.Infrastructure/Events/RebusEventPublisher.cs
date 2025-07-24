using SalesService.Application.Interfaces;
using SalesService.Domain.Events;
using Microsoft.Extensions.Logging;
using Rebus.Bus;

namespace SalesService.Infrastructure.Events;

/// <summary>
/// Rebus-based event publisher using In-Memory transport
/// This implementation publishes events to Rebus message bus for in-memory processing
/// </summary>
public class RebusEventPublisher : IEventPublisher
{
    private readonly IBus _bus;
    private readonly ILogger<RebusEventPublisher> _logger;

    public RebusEventPublisher(IBus bus, ILogger<RebusEventPublisher> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task PublishAsync(ISaleEvent @event, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "📢 Publishing domain event {EventType} for Sale {SaleId} via Rebus In-Memory",
                @event.GetType().Name,
                @event.SaleId);

            // Publish the event to Rebus In-Memory transport
            await _bus.Publish(@event);

            _logger.LogInformation(
                "✅ Successfully published {EventType} for Sale {SaleId}",
                @event.GetType().Name,
                @event.SaleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to publish event {EventType} for Sale {SaleId} via Rebus",
                @event.GetType().Name,
                @event.SaleId);
            
            // Re-throw to maintain the contract
            throw;
        }
    }
} 