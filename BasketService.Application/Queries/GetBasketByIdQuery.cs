using BasketService.Application.Interfaces;
using BasketService.Domain.Aggregates;
using MediatR;

namespace BasketService.Application.Queries;

public class GetBasketByIdQuery: IRequest<Basket?>
{
    public Guid UserId { get; set; }

    public GetBasketByIdQuery(Guid userId)
    {
        UserId = userId;
    }
}

public class GetBasketByIdHandler : IRequestHandler<GetBasketByIdQuery, Basket?>
{
    private readonly IAggregateRepository _repository;

    public GetBasketByIdHandler(IAggregateRepository repository)
    {
        _repository = repository;
    }

    public async Task<Basket?> Handle(GetBasketByIdQuery request, CancellationToken cancellationToken)
    {
        var basket = await _repository.LoadAsync<Basket>(request.UserId);

        return basket.Id == Guid.Empty ? null : basket;
    }
}