using MediatR;
using SalesService.Application.DTOs;
using SalesService.Application.Interfaces;
using AutoMapper;

namespace SalesService.Application.Queries;

public class GetSalesQueryHandler : IRequestHandler<GetSalesQuery, List<SaleDto>>
{
    private readonly ISaleRepository _repository;
    private readonly IMapper _mapper;

    public GetSalesQueryHandler(ISaleRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<SaleDto>> Handle(GetSalesQuery request, CancellationToken cancellationToken)
    {
        var sales = await _repository.GetAllAsync(cancellationToken);
        // TODO: Apply filtering, ordering, and pagination logic here
        return _mapper.Map<List<SaleDto>>(sales);
    }
} 