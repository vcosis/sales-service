using FluentAssertions;
using SalesService.Domain.Entities;
using Bogus;

namespace SalesService.Tests.Domain.Entities;

public class SaleItemTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void Constructor_WithValidData_ShouldCreateSaleItem()
    {
        // Arrange
        var productId = _faker.Random.Int(1, 100);
        var productName = _faker.Commerce.ProductName();
        var quantity = 5;
        var unitPrice = _faker.Random.Decimal(10, 1000);

        // Act
        var saleItem = new SaleItem(productId, productName, quantity, unitPrice);

        // Assert
        saleItem.ProductId.Should().Be(productId);
        saleItem.ProductName.Should().Be(productName);
        saleItem.Quantity.Should().Be(quantity);
        saleItem.UnitPrice.Should().Be(unitPrice);
        saleItem.Total.Should().Be((unitPrice * quantity) * 0.9m); // 10% discount for 5 items
        saleItem.Discount.Should().Be(unitPrice * quantity * 0.1m);
    }

    [Theory]
    [InlineData(1, 0.0)] // No discount
    [InlineData(3, 0.0)] // No discount
    [InlineData(4, 0.1)] // 10% discount
    [InlineData(9, 0.1)] // 10% discount
    [InlineData(10, 0.2)] // 20% discount
    [InlineData(15, 0.2)] // 20% discount
    [InlineData(20, 0.2)] // 20% discount
    public void Constructor_WithDifferentQuantities_ShouldApplyCorrectDiscounts(int quantity, decimal expectedDiscountRate)
    {
        // Arrange
        var productId = _faker.Random.Int(1, 100);
        var productName = _faker.Commerce.ProductName();
        var unitPrice = 100m;

        // Act
        var saleItem = new SaleItem(productId, productName, quantity, unitPrice);

        // Assert
        var expectedTotal = (unitPrice * quantity) * (1 - expectedDiscountRate);
        var expectedDiscount = unitPrice * quantity * expectedDiscountRate;
        
        saleItem.Total.Should().Be(expectedTotal);
        saleItem.Discount.Should().Be(expectedDiscount);
    }

    [Fact]
    public void UpdateItem_WithValidData_ShouldUpdatePropertiesAndRecalculateDiscount()
    {
        // Arrange
        var saleItem = new SaleItem(1, "Original Product", 5, 100m);
        var newProductId = _faker.Random.Int(1, 100);
        var newProductName = _faker.Commerce.ProductName();
        var newQuantity = 10;
        var newUnitPrice = 200m;

        // Act
        saleItem.UpdateItem(newProductId, newProductName, newQuantity, newUnitPrice);

        // Assert
        saleItem.ProductId.Should().Be(newProductId);
        saleItem.ProductName.Should().Be(newProductName);
        saleItem.Quantity.Should().Be(newQuantity);
        saleItem.UnitPrice.Should().Be(newUnitPrice);
        saleItem.Total.Should().Be((newUnitPrice * newQuantity) * 0.8m); // 20% discount for 10 items
        saleItem.Discount.Should().Be(newUnitPrice * newQuantity * 0.2m);
    }

    [Fact]
    public void UpdateItem_WithQuantityBelow4_ShouldRemoveDiscount()
    {
        // Arrange
        var saleItem = new SaleItem(1, "Product", 10, 100m); // Initially has 20% discount
        var newQuantity = 3;

        // Act
        saleItem.UpdateItem(1, "Product", newQuantity, 100m);

        // Assert
        saleItem.Total.Should().Be(100m * newQuantity); // No discount
        saleItem.Discount.Should().Be(0m);
    }

    [Fact]
    public void UpdateItem_WithQuantity4_ShouldApply10PercentDiscount()
    {
        // Arrange
        var saleItem = new SaleItem(1, "Product", 1, 100m); // Initially no discount
        var newQuantity = 4;

        // Act
        saleItem.UpdateItem(1, "Product", newQuantity, 100m);

        // Assert
        saleItem.Total.Should().Be((100m * newQuantity) * 0.9m); // 10% discount
        saleItem.Discount.Should().Be(100m * newQuantity * 0.1m);
    }

    [Fact]
    public void UpdateItem_WithQuantity10_ShouldApply20PercentDiscount()
    {
        // Arrange
        var saleItem = new SaleItem(1, "Product", 1, 100m); // Initially no discount
        var newQuantity = 10;

        // Act
        saleItem.UpdateItem(1, "Product", newQuantity, 100m);

        // Assert
        saleItem.Total.Should().Be((100m * newQuantity) * 0.8m); // 20% discount
        saleItem.Discount.Should().Be(100m * newQuantity * 0.2m);
    }

    [Fact]
    public void UpdateItem_WithQuantity20_ShouldApply20PercentDiscount()
    {
        // Arrange
        var saleItem = new SaleItem(1, "Product", 1, 100m); // Initially no discount
        var newQuantity = 20;

        // Act
        saleItem.UpdateItem(1, "Product", newQuantity, 100m);

        // Assert
        saleItem.Total.Should().Be((100m * newQuantity) * 0.8m); // 20% discount
        saleItem.Discount.Should().Be(100m * newQuantity * 0.2m);
    }
} 