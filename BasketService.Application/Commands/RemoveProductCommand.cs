using BasketService.Application.Interfaces;
using BasketService.Domain.Aggregates;
using MediatR;

namespace BasketService.Application.Commands;

public class RemoveProductCommand: IRequest<Guid>
{
    public Guid ProductId { get; set; }
    public Guid UserId { get; set; }
    public int Quantity { get; set; }
}

public class RemoveProductCommandHandler : IRequestHandler<RemoveProductCommand, Guid>
{
    private readonly IAggregateRepository _repository;

    public RemoveProductCommandHandler(IAggregateRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(RemoveProductCommand request, CancellationToken cancellationToken)
    {
        var basket = await _repository.LoadAsync<Basket>(request.UserId);
        basket.RemoveProduct(request.ProductId, request.Quantity);
        await _repository.SaveAsync(basket);
        
        return basket.Id;
    }
}