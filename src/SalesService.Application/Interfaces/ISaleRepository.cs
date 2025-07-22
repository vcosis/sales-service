using SalesService.Domain.Entities;

namespace SalesService.Application.Interfaces;

public interface ISaleRepository
{
    Task<Sale?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Sale>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Sale sale, CancellationToken cancellationToken = default);
    Task UpdateAsync(Sale sale, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
} 