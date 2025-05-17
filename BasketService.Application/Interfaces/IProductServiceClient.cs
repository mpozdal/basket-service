using BasketService.Application.DTOs;

namespace BasketService.Application.Interfaces;

public interface IProductServiceClient
{
    Task<ProductItemDto?> GetProductByIdAsync(Guid id);
    Task<bool> ReserveProductByIdAsync(Guid productId, int quantity);
    Task<bool> ReleaseProductByIdAsync(Guid productId, int quantity);
}