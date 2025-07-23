using MediatR;
using SalesService.Application.DTOs;
using SalesService.Application.Interfaces;
using AutoMapper;
using System.Linq;

namespace SalesService.Application.Queries;

public class GetSalesQueryHandler : IRequestHandler<GetSalesQuery, List<SaleDto>>
{
    private readonly ISaleRepository _repository;
    private readonly IMapper _mapper;

    public GetSalesQueryHandler(ISaleRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<SaleDto>> Handle(GetSalesQuery request, CancellationToken cancellationToken)
    {
        var sales = await _repository.GetAllAsync(cancellationToken);
        
        // Apply filtering
        var filteredSales = ApplyFilters(sales, request.Filter);
        
        // Apply ordering
        var orderedSales = ApplyOrdering(filteredSales, request.Order);
        
        // Apply pagination
        var paginatedSales = ApplyPagination(orderedSales, request.Page, request.Size);
        
        return _mapper.Map<List<SaleDto>>(paginatedSales);
    }

    private IEnumerable<Domain.Entities.Sale> ApplyFilters(IEnumerable<Domain.Entities.Sale> sales, string? filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
            return sales;

        var filters = filter.Split('&', StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var filterItem in filters)
        {
            var parts = filterItem.Split('=', 2);
            if (parts.Length != 2) continue;

            var field = parts[0].Trim();
            var value = parts[1].Trim();

            switch (field.ToLower())
            {
                case "cancelled":
                    if (bool.TryParse(value, out bool cancelled))
                    {
                        sales = sales.Where(s => s.Cancelled == cancelled);
                    }
                    break;
                case "customername":
                    if (value.StartsWith("*") && value.EndsWith("*"))
                    {
                        var searchTerm = value.Trim('*');
                        sales = sales.Where(s => s.CustomerName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
                    }
                    else if (value.StartsWith("*"))
                    {
                        var searchTerm = value.TrimStart('*');
                        sales = sales.Where(s => s.CustomerName.EndsWith(searchTerm, StringComparison.OrdinalIgnoreCase));
                    }
                    else if (value.EndsWith("*"))
                    {
                        var searchTerm = value.TrimEnd('*');
                        sales = sales.Where(s => s.CustomerName.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase));
                    }
                    else
                    {
                        sales = sales.Where(s => s.CustomerName.Equals(value, StringComparison.OrdinalIgnoreCase));
                    }
                    break;
                case "salenumber":
                    if (value.StartsWith("*") && value.EndsWith("*"))
                    {
                        var searchTerm = value.Trim('*');
                        sales = sales.Where(s => s.SaleNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
                    }
                    else if (value.StartsWith("*"))
                    {
                        var searchTerm = value.TrimStart('*');
                        sales = sales.Where(s => s.SaleNumber.EndsWith(searchTerm, StringComparison.OrdinalIgnoreCase));
                    }
                    else if (value.EndsWith("*"))
                    {
                        var searchTerm = value.TrimEnd('*');
                        sales = sales.Where(s => s.SaleNumber.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase));
                    }
                    else
                    {
                        sales = sales.Where(s => s.SaleNumber.Equals(value, StringComparison.OrdinalIgnoreCase));
                    }
                    break;
                case "totalamount":
                    if (decimal.TryParse(value, out decimal amount))
                    {
                        sales = sales.Where(s => s.TotalAmount == amount);
                    }
                    else if (value.Contains(">"))
                    {
                        var amountParts = value.Split('>');
                        if (amountParts.Length == 2 && decimal.TryParse(amountParts[1].Trim(), out decimal minAmount))
                        {
                            sales = sales.Where(s => s.TotalAmount > minAmount);
                        }
                    }
                    else if (value.Contains("<"))
                    {
                        var amountParts = value.Split('<');
                        if (amountParts.Length == 2 && decimal.TryParse(amountParts[1].Trim(), out decimal maxAmount))
                        {
                            sales = sales.Where(s => s.TotalAmount < maxAmount);
                        }
                    }
                    else if (value.Contains("-"))
                    {
                        var amountParts = value.Split('-');
                        if (amountParts.Length == 2 && decimal.TryParse(amountParts[0].Trim(), out decimal minAmount) && decimal.TryParse(amountParts[1].Trim(), out decimal maxAmount))
                        {
                            sales = sales.Where(s => s.TotalAmount >= minAmount && s.TotalAmount <= maxAmount);
                        }
                    }
                    break;
            }
        }

        return sales;
    }

    private IEnumerable<Domain.Entities.Sale> ApplyOrdering(IEnumerable<Domain.Entities.Sale> sales, string? order)
    {
        if (string.IsNullOrWhiteSpace(order))
            return sales.OrderBy(s => s.Id);

        var orderParts = order.Split(',', StringSplitOptions.RemoveEmptyEntries);
        IOrderedEnumerable<Domain.Entities.Sale>? orderedSales = null;

        foreach (var orderPart in orderParts)
        {
            var trimmedPart = orderPart.Trim();
            var spaceIndex = trimmedPart.LastIndexOf(' ');
            var field = spaceIndex > 0 ? trimmedPart.Substring(0, spaceIndex) : trimmedPart;
            var direction = spaceIndex > 0 ? trimmedPart.Substring(spaceIndex + 1) : "asc";

            switch (field.ToLower())
            {
                case "id":
                    orderedSales = direction.ToLower() == "desc" 
                        ? (orderedSales?.ThenByDescending(s => s.Id) ?? sales.OrderByDescending(s => s.Id))
                        : (orderedSales?.ThenBy(s => s.Id) ?? sales.OrderBy(s => s.Id));
                    break;
                case "date":
                    orderedSales = direction.ToLower() == "desc"
                        ? (orderedSales?.ThenByDescending(s => s.Date) ?? sales.OrderByDescending(s => s.Date))
                        : (orderedSales?.ThenBy(s => s.Date) ?? sales.OrderBy(s => s.Date));
                    break;
                case "customername":
                    orderedSales = direction.ToLower() == "desc"
                        ? (orderedSales?.ThenByDescending(s => s.CustomerName) ?? sales.OrderByDescending(s => s.CustomerName))
                        : (orderedSales?.ThenBy(s => s.CustomerName) ?? sales.OrderBy(s => s.CustomerName));
                    break;
                case "totalamount":
                    orderedSales = direction.ToLower() == "desc"
                        ? (orderedSales?.ThenByDescending(s => s.TotalAmount) ?? sales.OrderByDescending(s => s.TotalAmount))
                        : (orderedSales?.ThenBy(s => s.TotalAmount) ?? sales.OrderBy(s => s.TotalAmount));
                    break;
            }
        }

        return orderedSales ?? sales.OrderBy(s => s.Id);
    }

    private IEnumerable<Domain.Entities.Sale> ApplyPagination(IEnumerable<Domain.Entities.Sale> sales, int page, int size)
    {
        return sales.Skip((page - 1) * size).Take(size);
    }
} 