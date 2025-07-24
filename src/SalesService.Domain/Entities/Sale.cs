using SalesService.Domain.Events;

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

    // Domain Events
    private readonly List<ISaleEvent> _domainEvents = new();
    public IReadOnlyCollection<ISaleEvent> DomainEvents => _domainEvents.AsReadOnly();

    // Parameterless constructor for EF Core
    private Sale() { }

    /// <summary>
    /// Adds a domain event to the internal collection
    /// </summary>
    private void AddDomainEvent(ISaleEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Clears all domain events after they have been published
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

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

        // Add creation event
        AddDomainEvent(new SaleCreatedEvent(this));
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

        var item = new SaleItem(productId, productName, quantity, unitPrice);
        Items.Add(item);

        TotalAmount = Items.Sum(i => i.Total);

        // Add modification event
        AddDomainEvent(new SaleModifiedEvent(this, "Item added"));
    }

    /// <summary>
    /// Updates the sale properties.
    /// </summary>
    public void UpdateSale(
        string saleNumber,
        DateTime date,
        int customerId,
        string customerName,
        int branchId,
        string branchName)
    {
        SaleNumber = saleNumber;
        Date = date;
        CustomerId = customerId;
        CustomerName = customerName;
        BranchId = branchId;
        BranchName = branchName;

        // Add modification event
        AddDomainEvent(new SaleModifiedEvent(this, "Sale properties updated"));
    }

    /// <summary>
    /// Clears all items from the sale.
    /// </summary>
    public void ClearItems()
    {
        if (Items.Any())
        {
            // Add events for each item being removed
            foreach (var item in Items.ToList())
            {
                AddDomainEvent(new ItemCancelledEvent(this, item, "Item cleared"));
            }
        }

        Items.Clear();
        TotalAmount = 0;

        // Add modification event
        AddDomainEvent(new SaleModifiedEvent(this, "All items cleared"));
    }

    /// <summary>
    /// Cancels the sale.
    /// </summary>
    public void Cancel()
    {
        if (Cancelled)
            throw new InvalidOperationException("Sale is already cancelled.");
            
        Cancelled = true;

        // Add cancellation event
        AddDomainEvent(new SaleCancelledEvent(this));
    }



    /// <summary>
    /// Updates an existing item in the sale.
    /// </summary>
    public void UpdateItem(int itemId, int productId, string productName, int quantity, decimal unitPrice)
    {
        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new KeyNotFoundException($"Item with id {itemId} not found in sale.");

        if (quantity < 1)
            throw new ArgumentException("A quantidade deve ser pelo menos 1 unidade.");

        if (quantity > 20)
            throw new InvalidOperationException($"Não é possível adicionar mais de 20 unidades do produto '{productName}' em uma única venda. Quantidade solicitada: {quantity}.");

        item.UpdateItem(productId, productName, quantity, unitPrice);
        TotalAmount = Items.Sum(i => i.Total);

        // Add modification event
        AddDomainEvent(new SaleModifiedEvent(this, "Item updated"));
    }

    /// <summary>
    /// Removes an item from the sale by its ID.
    /// </summary>
    public void RemoveItem(int itemId)
    {
        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            Items.Remove(item);
            TotalAmount = Items.Sum(i => i.Total);

            // Add item cancelled event
            AddDomainEvent(new ItemCancelledEvent(this, item, "Item removed"));
        }
    }
} 