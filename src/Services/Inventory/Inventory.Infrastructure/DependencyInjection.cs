using Inventory.Application.Interfaces;
using Inventory.Infrastructure.BackgroundJobs;
using Inventory.Infrastructure.Persistence;
using Inventory.Infrastructure.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInventoryInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<InventoryDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("InventoryDb"),
                npgsqlOptions => npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "inventory")));

        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IStockReservationRepository, StockReservationRepository>();

        services.AddHostedService<ExpiredReservationCleanupJob>();

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration.GetConnectionString("RabbitMq"));
                cfg.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }
}
