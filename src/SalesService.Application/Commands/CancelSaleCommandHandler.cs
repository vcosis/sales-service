using MediatR;
using SalesService.Application.DTOs;
using SalesService.Application.Interfaces;
using AutoMapper;

namespace SalesService.Application.Commands;

public class CancelSaleCommandHandler : IRequestHandler<CancelSaleCommand, SaleDto>
{
    private readonly ISaleRepository _repository;
    private readonly IMapper _mapper;

    public CancelSaleCommandHandler(ISaleRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<SaleDto> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with id {request.Id} not found.");

        sale.Cancel();
        await _repository.UpdateAsync(sale, cancellationToken);
        return _mapper.Map<SaleDto>(sale);
    }
} 