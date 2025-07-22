using MediatR;
using SalesService.Application.DTOs;

namespace SalesService.Application.Queries;

public class GetSaleByIdQuery : IRequest<SaleDto>
{
    public int Id { get; set; }
    public GetSaleByIdQuery(int id) => Id = id;
} 