using MediatR;
using SalesService.Application.DTOs;
using SalesService.Application.Interfaces;
using SalesService.Domain.Entities;
using AutoMapper;

namespace SalesService.Application.Commands;

public class UpdateSaleCommandHandler : IRequestHandler<UpdateSaleCommand, SaleDto>
{
    private readonly ISaleRepository _repository;
    private readonly IMapper _mapper;

    public UpdateSaleCommandHandler(ISaleRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<SaleDto> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with id {request.Id} not found.");

        // Update sale properties
        // (Business rules for updating items can be added here)
        // For simplicity, replace all items
        sale = new Sale(
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

        await _repository.UpdateAsync(sale, cancellationToken);
        return _mapper.Map<SaleDto>(sale);
    }
} 