namespace BasketService.Domain.Events;

public record ProductAddedToBasket(Guid UserId, Guid ProductId, int Quantity);