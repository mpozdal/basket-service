using BasketService.Application.Interfaces;
using BasketService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BasketService.Infrastructure.Repositories;

public class ReservationRepository: IReservationRepository
{
    private readonly AppDbContext _dbContext;

    public ReservationRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<bool> CreateReservationAsync(BasketReservation reservation)
    {
       await _dbContext.Reservations.AddAsync(reservation);
       await _dbContext.SaveChangesAsync();
       
       return true;
    }

    public async Task<bool> UpdateReservationAsync(BasketReservation reservation)
    {
        var reservationToUpdate = await _dbContext.Reservations.FindAsync(reservation.UserId);
        if (reservationToUpdate == null)
            return false;
        
        reservationToUpdate.LastActivityAt = DateTime.UtcNow;
        _dbContext.Reservations.Update(reservationToUpdate);
        return true;

    }
    public async Task<List<Guid>> GetExpiredBasketsAsync(DateTime threshold)
    {
        return await _dbContext.Reservations
            .Where(x => x.LastActivityAt < threshold)
            .Select(x => x.UserId)
            .ToListAsync();
    }

}