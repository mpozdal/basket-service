using System.Text.Json;
using BasketService.Application.DTOs;
using BasketService.Application.Interfaces;

namespace BasketService.Infrastructure.ProductService;

public class ProductServiceClient: IProductServiceClient
{
    private readonly HttpClient _client;
    public ProductServiceClient(HttpClient client)
    {
        _client = client;
    }
    
    public async Task<ProductItemDto?> GetProductByIdAsync(Guid productId)
    {
        var response = await _client.GetAsync($"api/v1/products/{productId}");
        if (!response.IsSuccessStatusCode)
            return null;
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ProductItemDto>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
    public async Task<bool> ReserveProductByIdAsync(Guid productId, int quantity)
    {
        var response = await _client.PostAsync($"api/v1/products/{productId}/reserve?quantity={quantity}", null);
        if (!response.IsSuccessStatusCode)
            return false;
        return true;
        
    }
    public async Task<bool> ReleaseProductByIdAsync(Guid productId, int quantity)
    {
        var response = await _client.PostAsync($"api/v1/products/{productId}/release?quantity={quantity}", null);
        if (!response.IsSuccessStatusCode)
            return false;
        return true;
        
    }
}