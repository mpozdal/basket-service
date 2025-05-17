using BasketService.Application.DTOs;
using BasketService.Application.Interfaces;
using BasketService.Domain.Aggregates;
using MediatR;

namespace BasketService.Application.Commands;

public class AddProductCommand: IRequest<ProductItemDto?>
{
    public Guid ProductId { get; set; }
    public Guid UserId { get; set; }
    public int Quantity { get; set; }
}
public class AddProductCommandHandler : IRequestHandler<AddProductCommand, ProductItemDto?>
{
    private readonly IAggregateRepository _repository;
    private readonly IProductServiceClient _productServiceClient;
    
    public AddProductCommandHandler(IAggregateRepository repository, IProductServiceClient productServiceClient)
    {
        _repository = repository;
        _productServiceClient = productServiceClient;
    }
        
    public async Task<ProductItemDto?> Handle(AddProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productServiceClient.GetProductByIdAsync(request.ProductId);
            if (product == null)
            {
                return null;
            }

            var isReserved = await _productServiceClient.ReserveProductByIdAsync(request.ProductId, request.Quantity);
            if (!isReserved)
            {
                throw new Exception("Reservation failed");
            }

            var basket = await _repository.LoadAsync<Basket>(request.UserId);
            basket.AddProduct(product.Id, request.Quantity,  product.Price);
            await _repository.SaveAsync(basket);
            return product;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        
        
    }
}