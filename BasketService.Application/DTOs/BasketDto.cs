namespace BasketService.Application.DTOs;

public class BasketDto
{
    public Guid Id { get; set; }
    public bool IsFinalized { get; set; }
    public decimal TotalPrice { get; set; }
    public List<ProductItemDto> Items { get; set; } = new();
    public DateTime LastActivityAt { get; set; }
}