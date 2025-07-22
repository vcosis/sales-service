using MediatR;
using SalesService.Application.DTOs;

namespace SalesService.Application.Commands;

public class CreateSaleCommand : IRequest<SaleDto>
{
    public string SaleNumber { get; set; } = default!;
    public DateTime Date { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = default!;
    public int BranchId { get; set; }
    public string BranchName { get; set; } = default!;
    public List<CreateSaleItemDto> Items { get; set; } = new();
}

public class CreateSaleItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
} 