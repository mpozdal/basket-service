using BasketService.Application.Interfaces;
using BasketService.Domain.Aggregates;
using MediatR;

namespace BasketService.Application.Commands;

public class CreateBasketCommand: IRequest<Guid>
{
    public Guid UserId { get; set; }
}
public class CreateBasketCommandHandler : IRequestHandler<CreateBasketCommand, Guid>
{
    private readonly IAggregateRepository _repository;

    public CreateBasketCommandHandler(IAggregateRepository repository)
    {
        _repository = repository;
    }
        
    public async Task<Guid> Handle(CreateBasketCommand request, CancellationToken cancellationToken)
    {
        var basket = Basket.Create(request.UserId);
        await _repository.SaveAsync(basket);
        return basket.Id;
    }
}