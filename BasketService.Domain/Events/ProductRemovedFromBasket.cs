namespace BasketService.Domain.Events;

public record ProductRemovedFromBasket(Guid BasketId, Guid ProductId, int quantity);