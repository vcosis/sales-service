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
        sale.UpdateSale(
            request.SaleNumber,
            request.Date,
            request.CustomerId,
            request.CustomerName,
            request.BranchId,
            request.BranchName
        );

        // Get IDs of items to keep
        var itemIdsToKeep = request.Items.Where(i => i.Id > 0).Select(i => i.Id).ToList();
        
        // Remove items that are no longer in the list
        var itemsToRemove = sale.Items.Where(i => !itemIdsToKeep.Contains(i.Id)).ToList();
        foreach (var item in itemsToRemove)
        {
            sale.RemoveItem(item.Id);
        }

        // Update existing items and add new ones
        foreach (var itemDto in request.Items)
        {
            if (itemDto.Id > 0)
            {
                // Update existing item
                sale.UpdateItem(itemDto.Id, itemDto.ProductId, itemDto.ProductName, itemDto.Quantity, itemDto.UnitPrice);
            }
            else
            {
                // Add new item
                sale.AddItem(itemDto.ProductId, itemDto.ProductName, itemDto.Quantity, itemDto.UnitPrice);
            }
        }

        await _repository.UpdateAsync(sale, cancellationToken);
        return _mapper.Map<SaleDto>(sale);
    }
} 