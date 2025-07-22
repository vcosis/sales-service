using MediatR;
using SalesService.Application.DTOs;
using SalesService.Application.Interfaces;
using SalesService.Domain.Entities;
using AutoMapper;

namespace SalesService.Application.Commands;

public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, SaleDto>
{
    private readonly ISaleRepository _repository;
    private readonly IMapper _mapper;

    public CreateSaleCommandHandler(ISaleRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<SaleDto> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = new Sale(
            request.SaleNumber,
            request.Date,
            request.CustomerId,
            request.CustomerName,
            request.BranchId,
            request.BranchName,
            new List<SaleItem>()
        );

        foreach (var item in request.Items)
        {
            sale.AddItem(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice);
        }

        await _repository.AddAsync(sale, cancellationToken);
        return _mapper.Map<SaleDto>(sale);
    }
} 