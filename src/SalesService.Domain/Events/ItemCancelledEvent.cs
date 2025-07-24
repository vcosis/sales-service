using SalesService.Domain.Entities;

namespace SalesService.Domain.Events;

/// <summary>
/// Event raised when an item is cancelled/removed from a sale
/// </summary>
public class ItemCancelledEvent : ISaleEvent
{
    public int SaleId { get; }
    public DateTime OccurredAt { get; }
    public int ItemId { get; }
    public int ProductId { get; }
    public string ProductName { get; }
    public int Quantity { get; }
    public decimal UnitPrice { get; }
    public decimal TotalAmount { get; }
    public string CancellationReason { get; }

    public ItemCancelledEvent(Sale sale, SaleItem item, string cancellationReason = "Item removed")
    {
        SaleId = sale.Id;
        OccurredAt = DateTime.UtcNow;
        ItemId = item.Id;
        ProductId = item.ProductId;
        ProductName = item.ProductName;
        Quantity = item.Quantity;
        UnitPrice = item.UnitPrice;
        TotalAmount = item.Total;
        CancellationReason = cancellationReason;
    }
} 