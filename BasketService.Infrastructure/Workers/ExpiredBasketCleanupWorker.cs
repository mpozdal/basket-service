using BasketService.Application.Interfaces;
using BasketService.Domain.Aggregates;
using BasketService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BasketService.Infrastructure.Workers;

public class BasketCleanupWorker : BackgroundService
{
    private readonly IServiceProvider _provider;

    public BasketCleanupWorker(IServiceProvider provider)
    {
        _provider = provider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        { 
            using var scope = _provider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IReservationRepository>();

            var expirationThreshold = DateTime.UtcNow.AddMinutes(-15);
            var expiredBaskets = await repository.GetExpiredBasketsAsync(expirationThreshold);
            var aggregateRepo = scope.ServiceProvider.GetRequiredService<IAggregateRepository>();
            var productService = scope.ServiceProvider.GetRequiredService<IProductServiceClient>();

            foreach (var basketId in expiredBaskets)
            {
                var basket = await aggregateRepo.LoadAsync<Basket>(basketId);
                if (basket.IsFinalized) continue;
                foreach (var item in basket.Items)
                {
                    await productService.ReleaseProductByIdAsync(item.ProductId, item.Quantity);
                }
                
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
