namespace SalesService.Domain.Entities;

public class SaleItem
{
    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public string ProductName { get; private set; } = default!;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Discount { get; private set; }
    public decimal Total { get; private set; }

    public SaleItem(
        int productId,
        string productName,
        int quantity,
        decimal unitPrice,
        decimal discount)
    {
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Discount = discount;
        Total = CalculateTotal();
    }

    private decimal CalculateTotal()
    {
        return (UnitPrice * Quantity) - Discount;
    }
} 