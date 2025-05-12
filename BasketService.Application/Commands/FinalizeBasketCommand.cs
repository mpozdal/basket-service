using BasketService.Application.Interfaces;
using BasketService.Domain.Aggregates;
using MediatR;

namespace BasketService.Application.Commands;

public class FinalizeBasketCommand: IRequest<Guid>
{
    public Guid UserId { get; set; }
    
}

public class FinalizeBasketCommandHandler : IRequestHandler<FinalizeBasketCommand, Guid>
{
    private readonly IAggregateRepository _repository;

    public FinalizeBasketCommandHandler(IAggregateRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(FinalizeBasketCommand request, CancellationToken cancellationToken)
    {
        var basket = await _repository.LoadAsync<Basket>(request.UserId);
        basket.FinalizeBasket();
        await _repository.SaveAsync(basket);
        
        return basket.Id;
    }
}