using BasketService.Domain.Entities;
using BasketService.Domain.Events;
using BasketService.Domain.Framework;

namespace BasketService.Domain.Aggregates;

public class Basket: Aggregate
{
    private readonly List<ProductItem> _items = new();
    public bool IsFinalized { get; private set; }
    public decimal TotalPrice { get; private set; }

    public IReadOnlyCollection<ProductItem> Items => _items.AsReadOnly();
    
    public DateTime LastActivityAt { get; private set; }
    public static readonly TimeSpan ReservationTTL = TimeSpan.FromMinutes(15);
    public Basket() { }
    public static Basket Create(Guid userId)
    {
        if (Version >= 0)
        {
            throw new Exception();
        }
        var basket = new Basket();
        basket.Apply(new BasketCreated(userId));
        return basket;
    }
    public bool IsExpired() => DateTime.UtcNow - LastActivityAt > ReservationTTL;

    public void RefreshTimer()
    {
       Apply(new BasketRefreshed(DateTime.UtcNow));
    }
    public void AddProduct(Guid productId, int quantity, decimal price)
    {
        if (IsFinalized)
            throw new InvalidOperationException("Basket is finalized.");
        
        
        Apply(new ProductAddedToBasket(Id, productId, quantity, price));
    }
    public void RemoveProduct(Guid productId, int quantity)
    {
        if (IsFinalized)
            throw new InvalidOperationException("Basket is finalized.");

        Apply(new ProductRemovedFromBasket(Id, productId, quantity));
    }
    public void FinalizeBasket()
    {
        if (!_items.Any())
            throw new InvalidOperationException("Cannot finalize an empty basket.");

        if (IsFinalized)
            throw new InvalidOperationException("Basket is already finalized.");

        Apply(new BasketFinalized(Id));
    }
    protected override void When(object @event)
    {
        switch (@event)
        {
            case BasketCreated e:
                Id = e.UserId;
                IsFinalized = false;
                break;

            case ProductAddedToBasket e:
                var existing = _items.FirstOrDefault(i => i.ProductId == e.ProductId);
                LastActivityAt = DateTime.UtcNow;
                if (existing != null)
                {
                    existing.IncreaseQuantity(e.Quantity);
                }
                else
                {
                    _items.Add(new ProductItem(e.ProductId, e.Quantity, e.Price));
                }
                
                break;

            case ProductRemovedFromBasket e:
                var existingToRemove =  _items.FirstOrDefault(i => i.ProductId == e.ProductId);
                if(existingToRemove == null)
                    break;
                LastActivityAt = DateTime.UtcNow;
                if (existingToRemove.Quantity > e.quantity)
                {
                    existingToRemove.DecreaseQuantity(e.quantity);
                }
                else
                {
                    _items.RemoveAll(i => i.ProductId == e.ProductId);
                }
                
                break;
            case BasketRefreshed:
                LastActivityAt = DateTime.UtcNow;
                break;
            case BasketFinalized _:
                IsFinalized = true;
                break;
        }
    }
}