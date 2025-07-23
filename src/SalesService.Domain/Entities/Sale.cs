namespace SalesService.Domain.Entities;

public class Sale
{
    public int Id { get; private set; }
    public string SaleNumber { get; private set; } = default!;
    public DateTime Date { get; private set; }

    // External Identities for Customer
    public int CustomerId { get; private set; }
    public string CustomerName { get; private set; } = default!;

    // External Identities for Branch
    public int BranchId { get; private set; }
    public string BranchName { get; private set; } = default!;

    public decimal TotalAmount { get; private set; }
    public bool Cancelled { get; private set; }

    public List<SaleItem> Items { get; private set; } = new();

    // Parameterless constructor for EF Core
    private Sale() { }

    public Sale(
        string saleNumber,
        DateTime date,
        int customerId,
        string customerName,
        int branchId,
        string branchName,
        List<SaleItem> items)
    {
        SaleNumber = saleNumber;
        Date = date;
        CustomerId = customerId;
        CustomerName = customerName;
        BranchId = branchId;
        BranchName = branchName;
        Items = items ?? new List<SaleItem>();
        TotalAmount = Items.Sum(i => i.Total);
        Cancelled = false;
    }

    /// <summary>
    /// Adds an item to the sale, applying business rules for discounts and quantity limits.
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <param name="productName">Product name</param>
    /// <param name="quantity">Quantity of the product</param>
    /// <param name="unitPrice">Unit price of the product</param>
    /// <exception cref="ArgumentException">Thrown when quantity is less than 1</exception>
    /// <exception cref="InvalidOperationException">Thrown when quantity exceeds 20</exception>
    public void AddItem(int productId, string productName, int quantity, decimal unitPrice)
    {
        if (quantity < 1)
            throw new ArgumentException("A quantidade deve ser pelo menos 1 unidade.");

        if (quantity > 20)
            throw new InvalidOperationException($"Não é possível adicionar mais de 20 unidades do produto '{productName}' em uma única venda. Quantidade solicitada: {quantity}.");

        decimal discount = CalculateDiscount(quantity, unitPrice);

        var item = new SaleItem(productId, productName, quantity, unitPrice, discount);
        Items.Add(item);

        TotalAmount = Items.Sum(i => i.Total);
    }

    /// <summary>
    /// Calculates the discount based on quantity according to business rules.
    /// </summary>
    /// <param name="quantity">Quantity of the product</param>
    /// <param name="unitPrice">Unit price of the product</param>
    /// <returns>Discount value</returns>
    private decimal CalculateDiscount(int quantity, decimal unitPrice)
    {
        if (quantity >= 10 && quantity <= 20)
            return unitPrice * quantity * 0.20m; // 20% discount
        if (quantity >= 4)
            return unitPrice * quantity * 0.10m; // 10% discount
        return 0m; // No discount for less than 4
    }

    /// <summary>
    /// Cancels the sale.
    /// </summary>
    public void Cancel()
    {
        Cancelled = true;
    }
} 