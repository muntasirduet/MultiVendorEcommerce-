using Inventory.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.BackgroundJobs;

public class ExpiredReservationCleanupJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ExpiredReservationCleanupJob> _logger;
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(5);

    public ExpiredReservationCleanupJob(IServiceScopeFactory scopeFactory, ILogger<ExpiredReservationCleanupJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(Interval, stoppingToken);
            await ProcessExpiredReservationsAsync(stoppingToken);
        }
    }

    private async Task ProcessExpiredReservationsAsync(CancellationToken ct)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var reservationRepo = scope.ServiceProvider.GetRequiredService<IStockReservationRepository>();
            var inventoryRepo = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();

            var expiredReservations = await reservationRepo.GetExpiredReservationsAsync(ct);
            if (!expiredReservations.Any()) return;

            _logger.LogInformation("Processing {Count} expired reservations.", expiredReservations.Count);

            foreach (var reservation in expiredReservations)
            {
                var item = await inventoryRepo.GetByProductIdAsync(reservation.ProductId, ct);
                if (item is not null)
                {
                    item.ReleaseReservation(reservation.Quantity);
                    await inventoryRepo.UpdateAsync(item, ct);
                }
                reservation.MarkExpired();
                await reservationRepo.UpdateAsync(reservation, ct);
            }

            await inventoryRepo.SaveChangesAsync(ct);
            await reservationRepo.SaveChangesAsync(ct);

            _logger.LogInformation("Successfully processed {Count} expired reservations.", expiredReservations.Count);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error processing expired reservations.");
        }
    }
}
