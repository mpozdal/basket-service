using BasketService.Domain.Framework;

namespace BasketService.Application.Interfaces;

public interface IAggregateRepository
{
    Task SaveAsync<T>(T aggregate) where T : Aggregate;
    Task<T> LoadAsync<T>(Guid aggregateId) where T : Aggregate, new();
}