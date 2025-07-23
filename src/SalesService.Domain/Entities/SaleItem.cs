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

    // Parameterless constructor for EF Core
    private SaleItem() { }

    public SaleItem(
        int productId,
        string productName,
        int quantity,
        decimal unitPrice)
    {
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Discount = CalculateDiscount(quantity, unitPrice);
        Total = CalculateTotal();
    }

    private decimal CalculateTotal()
    {
        return (UnitPrice * Quantity) - Discount;
    }

    /// <summary>
    /// Updates the item properties and recalculates the total.
    /// </summary>
    public void UpdateItem(int productId, string productName, int quantity, decimal unitPrice)
    {
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Discount = CalculateDiscount(quantity, unitPrice);
        Total = CalculateTotal();
    }

    /// <summary>
    /// Calculates the discount based on quantity according to business rules.
    /// </summary>
    private decimal CalculateDiscount(int quantity, decimal unitPrice)
    {
        if (quantity >= 10 && quantity <= 20)
            return unitPrice * quantity * 0.20m; // 20% discount
        if (quantity >= 4)
            return unitPrice * quantity * 0.10m; // 10% discount
        return 0m; // No discount for less than 4
    }
} 