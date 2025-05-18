using BasketService.Application.DTOs;
using BasketService.Application.Interfaces;
using BasketService.Domain.Aggregates;
using BasketService.Domain.Entities;
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
    private readonly IReservationRepository _reservationRepository;
    
    public AddProductCommandHandler(IAggregateRepository repository, IProductServiceClient productServiceClient, IReservationRepository reservationRepository)
    {
        _repository = repository;
        _productServiceClient = productServiceClient;
        _reservationRepository = reservationRepository;
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
            if (basket.IsFinalized)
                throw new InvalidOperationException("Basket is finalized");

            if (basket.IsExpired())
                throw new InvalidOperationException("Basket expired");
            basket.RefreshTimer();
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