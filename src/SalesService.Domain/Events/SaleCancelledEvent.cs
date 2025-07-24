using SalesService.Domain.Entities;

namespace SalesService.Domain.Events;

/// <summary>
/// Event raised when a sale is cancelled
/// </summary>
public class SaleCancelledEvent : ISaleEvent
{
    public int SaleId { get; }
    public DateTime OccurredAt { get; }
    public string SaleNumber { get; }
    public DateTime SaleDate { get; }
    public int CustomerId { get; }
    public string CustomerName { get; }
    public int BranchId { get; }
    public string BranchName { get; }
    public decimal TotalAmount { get; }
    public int ItemsCount { get; }
    public DateTime CancelledAt { get; }

    public SaleCancelledEvent(Sale sale)
    {
        SaleId = sale.Id;
        OccurredAt = DateTime.UtcNow;
        SaleNumber = sale.SaleNumber;
        SaleDate = sale.Date;
        CustomerId = sale.CustomerId;
        CustomerName = sale.CustomerName;
        BranchId = sale.BranchId;
        BranchName = sale.BranchName;
        TotalAmount = sale.TotalAmount;
        ItemsCount = sale.Items.Count;
        CancelledAt = DateTime.UtcNow;
    }
} 