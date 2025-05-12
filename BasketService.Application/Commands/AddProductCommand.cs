using BasketService.Application.Interfaces;
using BasketService.Domain.Aggregates;
using MediatR;

namespace BasketService.Application.Commands;

public class AddProductCommand: IRequest<Guid>
{
    public Guid ProductId { get; set; }
    public Guid UserId { get; set; }
    public int Quantity { get; set; }
}
public class AddProductCommandHandler : IRequestHandler<AddProductCommand, Guid>
{
    private readonly IAggregateRepository _repository;
    
    public AddProductCommandHandler(IAggregateRepository repository)
    {
        _repository = repository;
    }
        
    public async Task<Guid> Handle(AddProductCommand request, CancellationToken cancellationToken)
    {
        var basket = await _repository.LoadAsync<Basket>(request.UserId);
        basket.AddProduct(request.ProductId, request.Quantity);
        await _repository.SaveAsync(basket);
        
        return basket.Id;
    }
}