using Identity.Application.Interfaces;
using Identity.Infrastructure.Keycloak;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<IdentityDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("IdentityDb"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "identity")));

        services.Configure<KeycloakSettings>(configuration.GetSection("Keycloak:Admin"));
        services.AddHttpClient<IKeycloakService, KeycloakAdminClient>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration.GetConnectionString("RabbitMq") ?? "amqp://localhost:5672/ecommerce");
                cfg.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }
}
