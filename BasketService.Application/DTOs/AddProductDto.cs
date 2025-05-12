namespace BasketService.Application.DTOs;

public class AddProductDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}