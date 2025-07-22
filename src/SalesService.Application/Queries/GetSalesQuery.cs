using MediatR;
using SalesService.Application.DTOs;

namespace SalesService.Application.Queries;

public class GetSalesQuery : IRequest<List<SaleDto>>
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? Order { get; set; }
    public string? Filter { get; set; }
} 