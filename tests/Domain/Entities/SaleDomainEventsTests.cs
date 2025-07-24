using FluentAssertions;
using SalesService.Domain.Entities;
using SalesService.Domain.Events;
using Bogus;

namespace SalesService.Tests.Domain.Entities;

public class SaleDomainEventsTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void Constructor_ShouldAddSaleCreatedEvent()
    {
        // Arrange & Act
        var sale = CreateValidSale();

        // Assert
        sale.DomainEvents.Should().HaveCount(1);
        sale.DomainEvents.First().Should().BeOfType<SaleCreatedEvent>();
        
        var createdEvent = (SaleCreatedEvent)sale.DomainEvents.First();
        createdEvent.SaleId.Should().Be(sale.Id);
        createdEvent.SaleNumber.Should().Be(sale.SaleNumber);
        createdEvent.CustomerName.Should().Be(sale.CustomerName);
    }

    [Fact]
    public void Cancel_ShouldAddSaleCancelledEvent()
    {
        // Arrange
        var sale = CreateValidSale();
        sale.ClearDomainEvents(); // Clear creation event

        // Act
        sale.Cancel();

        // Assert
        sale.DomainEvents.Should().HaveCount(1);
        sale.DomainEvents.First().Should().BeOfType<SaleCancelledEvent>();
        
        var cancelledEvent = (SaleCancelledEvent)sale.DomainEvents.First();
        cancelledEvent.SaleId.Should().Be(sale.Id);
        cancelledEvent.CancelledAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Cancel_WhenAlreadyCancelled_ShouldThrowException()
    {
        // Arrange
        var sale = CreateValidSale();
        sale.Cancel();
        sale.ClearDomainEvents();

        // Act & Assert
        var action = () => sale.Cancel();
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Sale is already cancelled.");
    }

    [Fact]
    public void AddItem_ShouldAddSaleModifiedEvent()
    {
        // Arrange
        var sale = CreateValidSale();
        sale.ClearDomainEvents(); // Clear creation event

        // Act
        sale.AddItem(1, "Product 1", 5, 100m);

        // Assert
        sale.DomainEvents.Should().HaveCount(1);
        sale.DomainEvents.First().Should().BeOfType<SaleModifiedEvent>();
        
        var modifiedEvent = (SaleModifiedEvent)sale.DomainEvents.First();
        modifiedEvent.SaleId.Should().Be(sale.Id);
        modifiedEvent.ModificationType.Should().Be("Item added");
    }

    [Fact]
    public void UpdateSale_ShouldAddSaleModifiedEvent()
    {
        // Arrange
        var sale = CreateValidSale();
        sale.ClearDomainEvents(); // Clear creation event

        // Act
        sale.UpdateSale("NEW001", DateTime.Now, 999, "New Customer", 99, "New Branch");

        // Assert
        sale.DomainEvents.Should().HaveCount(1);
        sale.DomainEvents.First().Should().BeOfType<SaleModifiedEvent>();
        
        var modifiedEvent = (SaleModifiedEvent)sale.DomainEvents.First();
        modifiedEvent.SaleId.Should().Be(sale.Id);
        modifiedEvent.ModificationType.Should().Be("Sale properties updated");
    }

    [Fact]
    public void RemoveItem_ShouldAddItemCancelledEvent()
    {
        // Arrange
        var sale = CreateValidSale();
        sale.AddItem(1, "Product 1", 5, 100m);
        sale.ClearDomainEvents(); // Clear previous events

        // Get the item ID from the sale
        var item = sale.Items.First();
        var itemId = item.Id;

        // Act
        sale.RemoveItem(itemId);

        // Assert
        sale.DomainEvents.Should().HaveCount(1);
        sale.DomainEvents.First().Should().BeOfType<ItemCancelledEvent>();
        
        var itemCancelledEvent = (ItemCancelledEvent)sale.DomainEvents.First();
        itemCancelledEvent.SaleId.Should().Be(sale.Id);
        itemCancelledEvent.ProductId.Should().Be(1);
        itemCancelledEvent.ProductName.Should().Be("Product 1");
        itemCancelledEvent.CancellationReason.Should().Be("Item removed");
    }

    [Fact]
    public void ClearItems_ShouldAddMultipleEvents()
    {
        // Arrange
        var sale = CreateValidSale();
        sale.AddItem(1, "Product 1", 5, 100m);
        sale.AddItem(2, "Product 2", 3, 200m);
        sale.ClearDomainEvents(); // Clear previous events

        // Act
        sale.ClearItems();

        // Assert
        sale.DomainEvents.Should().HaveCount(3); // 2 ItemCancelled + 1 SaleModified
        
        var itemCancelledEvents = sale.DomainEvents.OfType<ItemCancelledEvent>().ToList();
        itemCancelledEvents.Should().HaveCount(2);
        itemCancelledEvents.Should().OnlyContain(e => e.CancellationReason == "Item cleared");
        
        var saleModifiedEvent = sale.DomainEvents.OfType<SaleModifiedEvent>().Single();
        saleModifiedEvent.ModificationType.Should().Be("All items cleared");
    }

    [Fact]
    public void ClearDomainEvents_ShouldRemoveAllEvents()
    {
        // Arrange
        var sale = CreateValidSale();
        sale.AddItem(1, "Product 1", 5, 100m);
        
        sale.DomainEvents.Should().NotBeEmpty();

        // Act
        sale.ClearDomainEvents();

        // Assert
        sale.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void DomainEvents_ShouldBeReadOnly()
    {
        // Arrange
        var sale = CreateValidSale();
        var events = sale.DomainEvents;

        // Act & Assert
        events.Should().BeAssignableTo<IReadOnlyCollection<ISaleEvent>>();
        
        // Verify it's actually read-only by trying to cast
        var readOnlyCollection = events as IReadOnlyCollection<ISaleEvent>;
        readOnlyCollection.Should().NotBeNull();
        
        // The collection should be read-only (ReadOnlyCollection implements ICollection but is read-only)
        events.Should().BeOfType<System.Collections.ObjectModel.ReadOnlyCollection<ISaleEvent>>();
    }

    private Sale CreateValidSale()
    {
        return new Sale(
            _faker.Random.AlphaNumeric(10),
            _faker.Date.Recent(),
            _faker.Random.Int(1, 1000),
            _faker.Person.FullName,
            _faker.Random.Int(1, 100),
            _faker.Company.CompanyName(),
            new List<SaleItem>());
    }
} 