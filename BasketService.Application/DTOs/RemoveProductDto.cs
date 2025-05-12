namespace BasketService.Application.DTOs;

public class RemoveProductDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}