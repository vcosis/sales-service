using FluentAssertions;
using MediatR;
using NSubstitute;
using SalesService.Application.Queries;
using SalesService.Application.DTOs;
using SalesService.Application.Interfaces;
using SalesService.Domain.Entities;
using AutoMapper;
using Bogus;

namespace SalesService.Tests.Application.Queries;

public class GetSalesQueryHandlerTests
{
    private readonly ISaleRepository _repository;
    private readonly IMapper _mapper;
    private readonly GetSalesQueryHandler _handler;
    private readonly Faker _faker;

    public GetSalesQueryHandlerTests()
    {
        _repository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSalesQueryHandler(_repository, _mapper);
        _faker = new Faker();
    }

    [Fact]
    public async Task Handle_WithoutFilters_ShouldReturnAllSales()
    {
        // Arrange
        var sales = CreateSampleSales();
        var expectedDtos = CreateSampleSaleDtos();
        var query = new GetSalesQuery();

        _repository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(sales);
        _mapper.Map<List<SaleDto>>(Arg.Any<IEnumerable<Sale>>())
            .Returns(expectedDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(expectedDtos.Count);
        result.Should().BeEquivalentTo(expectedDtos);
    }

    [Fact]
    public async Task Handle_WithCancelledFilter_ShouldReturnOnlyCancelledSales()
    {
        // Arrange
        var sales = CreateSampleSales();
        var query = new GetSalesQuery { Filter = "cancelled=true" };
        var expectedDtos = CreateSampleSaleDtos().Where(d => d.Cancelled).ToList();

        _repository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(sales);
        _mapper.Map<List<SaleDto>>(Arg.Any<IEnumerable<Sale>>())
            .Returns(expectedDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(expectedDtos.Count);
        result.Should().OnlyContain(s => s.Cancelled);
    }

    [Fact]
    public async Task Handle_WithNotCancelledFilter_ShouldReturnOnlyActiveSales()
    {
        // Arrange
        var sales = CreateSampleSales();
        var query = new GetSalesQuery { Filter = "cancelled=false" };
        var expectedDtos = CreateSampleSaleDtos().Where(d => !d.Cancelled).ToList();

        _repository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(sales);
        _mapper.Map<List<SaleDto>>(Arg.Any<IEnumerable<Sale>>())
            .Returns(expectedDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(expectedDtos.Count);
        result.Should().OnlyContain(s => !s.Cancelled);
    }

    [Fact]
    public async Task Handle_WithCustomerNameFilter_ShouldReturnMatchingSales()
    {
        // Arrange
        var sales = CreateSampleSales();
        var query = new GetSalesQuery { Filter = "customername=John*" };
        var expectedDtos = CreateSampleSaleDtos().Where(d => d.CustomerName.StartsWith("John")).ToList();

        _repository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(sales);
        _mapper.Map<List<SaleDto>>(Arg.Any<IEnumerable<Sale>>())
            .Returns(expectedDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(expectedDtos.Count);
        result.Should().OnlyContain(s => s.CustomerName.StartsWith("John"));
    }

    [Fact]
    public async Task Handle_WithMultipleFilters_ShouldApplyAllFilters()
    {
        // Arrange
        var sales = CreateSampleSales();
        var query = new GetSalesQuery { Filter = "cancelled=false&customername=John*" };
        var expectedDtos = CreateSampleSaleDtos().Where(d => !d.Cancelled && d.CustomerName.StartsWith("John")).ToList();

        _repository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(sales);
        _mapper.Map<List<SaleDto>>(Arg.Any<IEnumerable<Sale>>())
            .Returns(expectedDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(expectedDtos.Count);
        result.Should().OnlyContain(s => !s.Cancelled && s.CustomerName.StartsWith("John"));
    }

    [Fact]
    public async Task Handle_WithOrdering_ShouldReturnOrderedResults()
    {
        // Arrange
        var sales = CreateSampleSales();
        var query = new GetSalesQuery { Order = "date desc" };
        var expectedDtos = CreateSampleSaleDtos().OrderByDescending(d => d.Date).ToList();

        _repository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(sales);
        _mapper.Map<List<SaleDto>>(Arg.Any<IEnumerable<Sale>>())
            .Returns(expectedDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(expectedDtos.Count);
    }

    [Fact]
    public async Task Handle_WithPagination_ShouldReturnPaginatedResults()
    {
        // Arrange
        var sales = CreateSampleSales();
        var query = new GetSalesQuery { Page = 2, Size = 2 };
        var expectedDtos = CreateSampleSaleDtos().Skip(2).Take(2).ToList();

        _repository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(sales);
        _mapper.Map<List<SaleDto>>(Arg.Any<IEnumerable<Sale>>())
            .Returns(expectedDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(expectedDtos.Count);
    }

    [Fact]
    public async Task Handle_WithInvalidFilter_ShouldIgnoreInvalidFilter()
    {
        // Arrange
        var sales = CreateSampleSales();
        var query = new GetSalesQuery { Filter = "invalid=value" };
        var expectedDtos = CreateSampleSaleDtos();

        _repository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(sales);
        _mapper.Map<List<SaleDto>>(Arg.Any<IEnumerable<Sale>>())
            .Returns(expectedDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(expectedDtos.Count);
    }

    private List<Sale> CreateSampleSales()
    {
        return new List<Sale>
        {
            CreateSale(1, "SALE-001", "John Doe", false, 1000m),
            CreateSale(2, "SALE-002", "Jane Smith", false, 2000m),
            CreateSale(3, "SALE-003", "John Wilson", true, 1500m),
            CreateSale(4, "SALE-004", "Mary Johnson", false, 3000m),
            CreateSale(5, "SALE-005", "John Brown", false, 2500m)
        };
    }

    private List<SaleDto> CreateSampleSaleDtos()
    {
        return new List<SaleDto>
        {
            CreateSaleDto(1, "SALE-001", "John Doe", false, 1000m),
            CreateSaleDto(2, "SALE-002", "Jane Smith", false, 2000m),
            CreateSaleDto(3, "SALE-003", "John Wilson", true, 1500m),
            CreateSaleDto(4, "SALE-004", "Mary Johnson", false, 3000m),
            CreateSaleDto(5, "SALE-005", "John Brown", false, 2500m)
        };
    }

    private Sale CreateSale(int id, string saleNumber, string customerName, bool cancelled, decimal totalAmount)
    {
        var sale = new Sale(
            saleNumber,
            _faker.Date.Recent(),
            _faker.Random.Int(1, 1000),
            customerName,
            _faker.Random.Int(1, 100),
            _faker.Company.CompanyName(),
            new List<SaleItem>());

        // Set the ID using reflection since it's private set
        var idProperty = typeof(Sale).GetProperty("Id");
        idProperty?.SetValue(sale, id);

        if (cancelled)
        {
            sale.Cancel();
        }

        // Set TotalAmount using reflection
        var totalAmountProperty = typeof(Sale).GetProperty("TotalAmount");
        totalAmountProperty?.SetValue(sale, totalAmount);

        return sale;
    }

    private SaleDto CreateSaleDto(int id, string saleNumber, string customerName, bool cancelled, decimal totalAmount)
    {
        return new SaleDto
        {
            Id = id,
            SaleNumber = saleNumber,
            Date = _faker.Date.Recent(),
            CustomerId = _faker.Random.Int(1, 1000),
            CustomerName = customerName,
            BranchId = _faker.Random.Int(1, 100),
            BranchName = _faker.Company.CompanyName(),
            TotalAmount = totalAmount,
            Cancelled = cancelled,
            Items = new List<SaleItemDto>()
        };
    }
} 