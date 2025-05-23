namespace BasketService.Application.DTOs;

public class ProductItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid CategoryId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int ReservedStock { get; set; }
}