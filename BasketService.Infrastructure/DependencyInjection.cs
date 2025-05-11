using BasketService.Infrastructure.Repositories;
using EventStore.ClientAPI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BasketService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var eventStoreConnection = EventStoreConnection.Create(
            connectionString: configuration.GetValue<string>("EventStore:ConnectionString"),
            builder: ConnectionSettings.Create().KeepReconnecting(),
            connectionName: configuration.GetValue<string>("EventStore:ConnectionName"));

        eventStoreConnection.ConnectAsync().GetAwaiter().GetResult();

        services.AddSingleton(eventStoreConnection);
        services.AddTransient<AggregateRepository>();

        return services;
    }
}