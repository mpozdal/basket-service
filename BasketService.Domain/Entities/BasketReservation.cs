namespace BasketService.Domain.Entities;

public class BasketReservation
{
    public Guid Id {get; set;}
    public Guid UserId { get; set; }
    public DateTime LastActivityAt { get; set; }
}