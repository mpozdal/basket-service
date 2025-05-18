using BasketService.Application.Interfaces;
using BasketService.Domain.Aggregates;
using MediatR;

namespace BasketService.Application.Commands;

public class RemoveProductCommand: IRequest<bool>
{
    public Guid ProductId { get; set; }
    public Guid UserId { get; set; }
    public int Quantity { get; set; }
}

public class RemoveProductCommandHandler : IRequestHandler<RemoveProductCommand, bool>
{
    private readonly IAggregateRepository _repository;
    private readonly IProductServiceClient _productServiceClient;

    public RemoveProductCommandHandler(IAggregateRepository repository, IProductServiceClient productServiceClient)
    {
        _repository = repository;
        _productServiceClient = productServiceClient;
    }

    public async Task<bool> Handle(RemoveProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Quantity <= 0)
                throw new Exception("Quantity must be greater than zero");
            var product = await _productServiceClient.GetProductByIdAsync(request.ProductId);

            if (product == null)
                throw new ArgumentException("Product not found.");
            
            var result = await _productServiceClient.ReleaseProductByIdAsync(request.ProductId, request.Quantity);
            if (!result)
                throw new Exception("Product has not been released.");

            var basket = await _repository.LoadAsync<Basket>(request.UserId);
            if (basket.IsFinalized)
                throw new InvalidOperationException("Basket is finalized");

            if (basket.IsExpired())
                throw new InvalidOperationException("Basket expired");
            basket.RefreshTimer();
            basket.RemoveProduct(request.ProductId, request.Quantity);
            await _repository.SaveAsync(basket);

            return true;
        }
        catch (Exception e)
        {
            throw new Exception("Error removing product", e);
        }
    }
}