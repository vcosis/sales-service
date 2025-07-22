using MediatR;
using SalesService.Application.DTOs;
using SalesService.Application.Interfaces;
using AutoMapper;

namespace SalesService.Application.Queries;

public class GetSaleByIdQueryHandler : IRequestHandler<GetSaleByIdQuery, SaleDto>
{
    private readonly ISaleRepository _repository;
    private readonly IMapper _mapper;

    public GetSaleByIdQueryHandler(ISaleRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<SaleDto> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
    {
        var sale = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with id {request.Id} not found.");

        return _mapper.Map<SaleDto>(sale);
    }
} 