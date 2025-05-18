using BasketService.Domain.Entities;

namespace BasketService.Application.Interfaces;

public interface IReservationRepository
{
    Task<bool> CreateReservationAsync(BasketReservation reservation);
    Task<bool> UpdateReservationAsync(BasketReservation reservation);
    Task<List<Guid>> GetExpiredBasketsAsync(DateTime threshold);

}