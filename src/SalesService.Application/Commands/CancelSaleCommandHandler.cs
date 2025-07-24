using MediatR;
using SalesService.Application.DTOs;
using SalesService.Application.Interfaces;
using SalesService.Domain.Events;
using AutoMapper;

namespace SalesService.Application.Commands;

public class CancelSaleCommandHandler : IRequestHandler<CancelSaleCommand, SaleDto>
{
    private readonly ISaleRepository _repository;
    private readonly IMapper _mapper;
    private readonly IEventPublisher _eventPublisher;

    public CancelSaleCommandHandler(ISaleRepository repository, IMapper mapper, IEventPublisher eventPublisher)
    {
        _repository = repository;
        _mapper = mapper;
        _eventPublisher = eventPublisher;
    }

    public async Task<SaleDto> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with id {request.Id} not found.");

        sale.Cancel();
        await _repository.UpdateAsync(sale, cancellationToken);
        
        // Publish domain events
        foreach (var domainEvent in sale.DomainEvents)
        {
            await _eventPublisher.PublishAsync(domainEvent, cancellationToken);
        }
        
        // Clear events after publishing
        sale.ClearDomainEvents();
        
        return _mapper.Map<SaleDto>(sale);
    }
} 