using BasketService.Application.DTOs;

namespace BasketService.Application.Interfaces;

public interface IProductServiceClient
{
    Task<ProductItemDto?> GetProductByIdAsync(Guid id);
}