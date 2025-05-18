using BasketService.Application.DTOs;
using BasketService.Application.Interfaces;
using BasketService.Domain.Aggregates;
using MediatR;

namespace BasketService.Application.Queries;

public class GetBasketByIdQuery: IRequest<BasketDto?>
{
    public Guid UserId { get; set; }

    public GetBasketByIdQuery(Guid userId)
    {
        UserId = userId;
    }
}

public class GetBasketByIdHandler : IRequestHandler<GetBasketByIdQuery, BasketDto?>
{
    private readonly IAggregateRepository _repository;
    private readonly IProductServiceClient _productServiceClient;

    public GetBasketByIdHandler(IAggregateRepository repository, IProductServiceClient productServiceClient)
    {
        _repository = repository;
        _productServiceClient = productServiceClient;
    }

    public async Task<BasketDto?> Handle(GetBasketByIdQuery request, CancellationToken cancellationToken)
    {
        var basket = await _repository.LoadAsync<Basket>(request.UserId);

        if (basket.Id == Guid.Empty)
            return null;

        var dto = new BasketDto()
        {
            Id = basket.Id,
            IsFinalized = basket.IsFinalized,
            Items = new List<ProductItemDto>(),
            LastActivityAt = basket.LastActivityAt
        };
        
        foreach (var item in basket.Items)
        {
            var product = await _productServiceClient.GetProductByIdAsync(item.ProductId);
            if (product == null)
                continue;
            dto.Items.Add(new ProductItemDto()
            {
            Id = item.ProductId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = item.Quantity,
            CategoryId = product.CategoryId,
            Stock = product.Stock - product.ReservedStock
            });
            
        }
        dto.TotalPrice = dto.Items.Sum(x => x.Quantity * x.Price);

        return dto;
    }
}