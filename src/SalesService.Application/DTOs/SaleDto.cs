namespace SalesService.Application.DTOs;

public class SaleDto
{
    public int Id { get; set; }
    public string SaleNumber { get; set; } = default!;
    public DateTime Date { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = default!;
    public int BranchId { get; set; }
    public string BranchName { get; set; } = default!;
    public decimal TotalAmount { get; set; }
    public bool Cancelled { get; set; }
    public List<SaleItemDto> Items { get; set; } = new();
} 