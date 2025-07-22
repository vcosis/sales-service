using MediatR;
using SalesService.Application.DTOs;

namespace SalesService.Application.Commands;

public class CancelSaleCommand : IRequest<SaleDto>
{
    public int Id { get; set; }
    public CancelSaleCommand(int id) => Id = id;
} 