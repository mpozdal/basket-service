using BasketService.Application.Interfaces;
using BasketService.Domain.Entities;
using BasketService.Infrastructure.Repositories;
using BasketService.Infrastructure.Workers;
using EventStore.ClientAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BasketService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        services.AddTransient<IAggregateRepository, AggregateRepository>();
        services.AddHostedService<BasketCleanupWorker>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        return services;
    }


}