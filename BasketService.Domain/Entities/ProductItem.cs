namespace BasketService.Domain.Entities;

public class ProductItem
{
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }

    public ProductItem(Guid productId, int quantity, decimal price)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        ProductId = productId;
        Quantity = quantity;
        Price = price;
    }

    public void IncreaseQuantity(int amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive.");
        Quantity += amount;
    }
    public void DecreaseQuantity(int amount)
    {
        if (Quantity < amount)
            throw new ArgumentException("Amount must be positive.");
        Quantity -= amount;
    }

}