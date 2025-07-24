using FluentAssertions;
using SalesService.Domain.Entities;
using Bogus;

namespace SalesService.Tests.Domain.Entities;

public class SaleTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void Constructor_WithValidData_ShouldCreateSale()
    {
        // Arrange
        var saleNumber = _faker.Random.AlphaNumeric(10);
        var date = _faker.Date.Recent();
        var customerId = _faker.Random.Int(1, 1000);
        var customerName = _faker.Person.FullName;
        var branchId = _faker.Random.Int(1, 100);
        var branchName = _faker.Company.CompanyName();
        var items = new List<SaleItem>();

        // Act
        var sale = new Sale(saleNumber, date, customerId, customerName, branchId, branchName, items);

        // Assert
        sale.SaleNumber.Should().Be(saleNumber);
        sale.Date.Should().Be(date);
        sale.CustomerId.Should().Be(customerId);
        sale.CustomerName.Should().Be(customerName);
        sale.BranchId.Should().Be(branchId);
        sale.BranchName.Should().Be(branchName);
        sale.TotalAmount.Should().Be(0);
        sale.Cancelled.Should().BeFalse();
        sale.Items.Should().BeEmpty();
    }

    [Fact]
    public void AddItem_WithValidQuantity_ShouldAddItemAndUpdateTotal()
    {
        // Arrange
        var sale = CreateValidSale();
        var productId = _faker.Random.Int(1, 100);
        var productName = _faker.Commerce.ProductName();
        var quantity = 5;
        var unitPrice = _faker.Random.Decimal(10, 1000);

        // Act
        sale.AddItem(productId, productName, quantity, unitPrice);

        // Assert
        sale.Items.Should().HaveCount(1);
        sale.Items.First().ProductId.Should().Be(productId);
        sale.Items.First().ProductName.Should().Be(productName);
        sale.Items.First().Quantity.Should().Be(quantity);
        sale.Items.First().UnitPrice.Should().Be(unitPrice);
        sale.TotalAmount.Should().Be(quantity * unitPrice * 0.9m); // 10% discount for 5 items
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AddItem_WithInvalidQuantity_ShouldThrowArgumentException(int invalidQuantity)
    {
        // Arrange
        var sale = CreateValidSale();
        var productId = _faker.Random.Int(1, 100);
        var productName = _faker.Commerce.ProductName();
        var unitPrice = _faker.Random.Decimal(10, 1000);

        // Act & Assert
        var action = () => sale.AddItem(productId, productName, invalidQuantity, unitPrice);
        action.Should().Throw<ArgumentException>()
            .WithMessage("A quantidade deve ser pelo menos 1 unidade.");
    }

    [Fact]
    public void AddItem_WithQuantityAbove20_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var sale = CreateValidSale();
        var productId = _faker.Random.Int(1, 100);
        var productName = _faker.Commerce.ProductName();
        var quantity = 21;
        var unitPrice = _faker.Random.Decimal(10, 1000);

        // Act & Assert
        var action = () => sale.AddItem(productId, productName, quantity, unitPrice);
        action.Should().Throw<InvalidOperationException>()
            .WithMessage($"Não é possível adicionar mais de 20 unidades do produto '{productName}' em uma única venda. Quantidade solicitada: {quantity}.");
    }

    [Theory]
    [InlineData(1, 0.0)] // No discount
    [InlineData(3, 0.0)] // No discount
    [InlineData(4, 0.1)] // 10% discount
    [InlineData(9, 0.1)] // 10% discount
    [InlineData(10, 0.2)] // 20% discount
    [InlineData(15, 0.2)] // 20% discount
    [InlineData(20, 0.2)] // 20% discount
    public void AddItem_WithDifferentQuantities_ShouldApplyCorrectDiscounts(int quantity, decimal expectedDiscountRate)
    {
        // Arrange
        var sale = CreateValidSale();
        var productId = _faker.Random.Int(1, 100);
        var productName = _faker.Commerce.ProductName();
        var unitPrice = 100m;

        // Act
        sale.AddItem(productId, productName, quantity, unitPrice);

        // Assert
        var item = sale.Items.First();
        var expectedTotal = (unitPrice * quantity) * (1 - expectedDiscountRate);
        item.Total.Should().Be(expectedTotal);
        item.Discount.Should().Be(unitPrice * quantity * expectedDiscountRate);
    }

    [Fact]
    public void UpdateSale_WithValidData_ShouldUpdateProperties()
    {
        // Arrange
        var sale = CreateValidSale();
        var newSaleNumber = _faker.Random.AlphaNumeric(10);
        var newDate = _faker.Date.Recent();
        var newCustomerId = _faker.Random.Int(1, 1000);
        var newCustomerName = _faker.Person.FullName;
        var newBranchId = _faker.Random.Int(1, 100);
        var newBranchName = _faker.Company.CompanyName();

        // Act
        sale.UpdateSale(newSaleNumber, newDate, newCustomerId, newCustomerName, newBranchId, newBranchName);

        // Assert
        sale.SaleNumber.Should().Be(newSaleNumber);
        sale.Date.Should().Be(newDate);
        sale.CustomerId.Should().Be(newCustomerId);
        sale.CustomerName.Should().Be(newCustomerName);
        sale.BranchId.Should().Be(newBranchId);
        sale.BranchName.Should().Be(newBranchName);
    }

    [Fact]
    public void UpdateItem_WithValidData_ShouldUpdateItemAndRecalculateTotal()
    {
        // Arrange
        var sale = CreateValidSale();
        var productId = _faker.Random.Int(1, 100);
        var productName = _faker.Commerce.ProductName();
        sale.AddItem(productId, productName, 5, 100m);

        var item = sale.Items.First();
        var newProductId = _faker.Random.Int(1, 100);
        var newProductName = _faker.Commerce.ProductName();
        var newQuantity = 10;
        var newUnitPrice = 200m;

        // Act
        sale.UpdateItem(item.Id, newProductId, newProductName, newQuantity, newUnitPrice);

        // Assert
        item.ProductId.Should().Be(newProductId);
        item.ProductName.Should().Be(newProductName);
        item.Quantity.Should().Be(newQuantity);
        item.UnitPrice.Should().Be(newUnitPrice);
        item.Total.Should().Be((newUnitPrice * newQuantity) * 0.8m); // 20% discount for 10 items
    }

    [Fact]
    public void UpdateItem_WithNonExistentItem_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var sale = CreateValidSale();
        var nonExistentItemId = 999;

        // Act & Assert
        var action = () => sale.UpdateItem(nonExistentItemId, 1, "Product", 5, 100m);
        action.Should().Throw<KeyNotFoundException>()
            .WithMessage($"Item with id {nonExistentItemId} not found in sale.");
    }

    [Fact]
    public void RemoveItem_WithValidItemId_ShouldRemoveItemAndUpdateTotal()
    {
        // Arrange
        var sale = CreateValidSale();
        sale.AddItem(1, "Product 1", 5, 100m);
        sale.AddItem(2, "Product 2", 3, 50m);

        var initialTotal = sale.TotalAmount;
        var itemToRemove = sale.Items.First();

        // Act
        sale.RemoveItem(itemToRemove.Id);

        // Assert
        sale.Items.Should().HaveCount(1);
        sale.Items.Should().NotContain(itemToRemove);
        sale.TotalAmount.Should().BeLessThan(initialTotal);
    }

    [Fact]
    public void RemoveItem_WithNonExistentItemId_ShouldNotThrowException()
    {
        // Arrange
        var sale = CreateValidSale();
        var nonExistentItemId = 999;

        // Act & Assert
        var action = () => sale.RemoveItem(nonExistentItemId);
        action.Should().NotThrow();
    }

    [Fact]
    public void ClearItems_ShouldRemoveAllItemsAndSetTotalToZero()
    {
        // Arrange
        var sale = CreateValidSale();
        sale.AddItem(1, "Product 1", 5, 100m);
        sale.AddItem(2, "Product 2", 3, 50m);

        // Act
        sale.ClearItems();

        // Assert
        sale.Items.Should().BeEmpty();
        sale.TotalAmount.Should().Be(0);
    }

    [Fact]
    public void Cancel_ShouldSetCancelledToTrue()
    {
        // Arrange
        var sale = CreateValidSale();

        // Act
        sale.Cancel();

        // Assert
        sale.Cancelled.Should().BeTrue();
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