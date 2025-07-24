using SalesService.Domain.Events;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;

namespace SalesService.Application.EventHandlers;

/// <summary>
/// Handles SaleCreatedEvent via Rebus message bus
/// This demonstrates how domain events can be processed asynchronously
/// </summary>
public class SaleCreatedEventHandler : IHandleMessages<SaleCreatedEvent>
{
    private readonly ILogger<SaleCreatedEventHandler> _logger;

    public SaleCreatedEventHandler(ILogger<SaleCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(SaleCreatedEvent message)
    {
        _logger.LogInformation(
            "🎉 Processing SaleCreatedEvent: Sale {SaleNumber} created for {CustomerName}",
            message.SaleNumber,
            message.CustomerName);

        // Simulate some async processing
        await Task.Delay(100);

        _logger.LogInformation(
            "✅ SaleCreatedEvent processed successfully: Sale {SaleNumber}",
            message.SaleNumber);

        // Here you could:
        // - Send notifications
        // - Update analytics
        // - Trigger inventory updates
        // - Send to external systems
    }
} 