using BasketService.Application.Interfaces;
using BasketService.Infrastructure.Repositories;
using EventStore.ClientAPI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BasketService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
     
        services.AddTransient<IAggregateRepository, AggregateRepository>();

        return services;
    }


}