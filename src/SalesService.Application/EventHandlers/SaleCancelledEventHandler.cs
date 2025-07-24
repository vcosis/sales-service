using SalesService.Domain.Events;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;

namespace SalesService.Application.EventHandlers;

/// <summary>
/// Handles SaleCancelledEvent via Rebus message bus
/// This demonstrates how domain events can be processed asynchronously
/// </summary>
public class SaleCancelledEventHandler : IHandleMessages<SaleCancelledEvent>
{
    private readonly ILogger<SaleCancelledEventHandler> _logger;

    public SaleCancelledEventHandler(ILogger<SaleCancelledEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(SaleCancelledEvent message)
    {
        _logger.LogInformation(
            "❌ Processing SaleCancelledEvent: Sale {SaleNumber} cancelled for {CustomerName}",
            message.SaleNumber,
            message.CustomerName);

        // Simulate some async processing
        await Task.Delay(100);

        _logger.LogInformation(
            "✅ SaleCancelledEvent processed successfully: Sale {SaleNumber}",
            message.SaleNumber);

        // Here you could:
        // - Restore inventory
        // - Process refunds
        // - Send cancellation notifications
        // - Update analytics
        // - Notify external systems
    }
} 