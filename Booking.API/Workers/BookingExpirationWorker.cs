using System.Net.Http.Headers;
using System.Net.Http.Json;
using Booking.API.Data;
using Booking.API.Domain;
using Booking.API.Services;
using Microsoft.EntityFrameworkCore;

namespace Booking.API.Workers;

public class BookingExpirationWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BookingExpirationWorker> _logger;
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(1);

    public BookingExpirationWorker(
        IServiceProvider serviceProvider,
        ILogger<BookingExpirationWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Booking Expiration Worker started. Interval: {Interval}", Interval);

        await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessExpiredBookingsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Booking Expiration Worker iteration");
            }

            await Task.Delay(Interval, stoppingToken);
        }

        _logger.LogInformation("Booking Expiration Worker stopping.");
    }

    private async Task ProcessExpiredBookingsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
        var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        var tokenProvider = scope.ServiceProvider.GetRequiredService<IServiceTokenProvider>();

        var now = DateTime.UtcNow;

        var expiredBookings = await context.Bookings
            .Include(b => b.Seats)
            .Where(b => b.Status == BookingStatus.Pending && b.ExpiresAt < now)
            .ToListAsync(cancellationToken);

        if (expiredBookings.Count == 0)
            return;

        _logger.LogInformation("Found {Count} expired booking(s) to process.", expiredBookings.Count);

        var httpClient = httpClientFactory.CreateClient("EventCatalog");
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenProvider.GetServiceToken());

        foreach (var booking in expiredBookings)
        {
            try
            {
                var seatIds = booking.Seats.Select(s => s.SeatId).ToList();

                var response = await httpClient.PostAsJsonAsync(
                    $"/api/events/{booking.EventId}/seats/release",
                    seatIds,
                    cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    booking.Status = BookingStatus.Expired;
                    _logger.LogInformation(
                        "Expired booking {BookingId} (was pending for user {UserId}). Seats released.",
                        booking.Id, booking.UserId);
                }
                else
                {
                    _logger.LogWarning(
                        "Failed to release seats for booking {BookingId}. Status: {StatusCode}",
                        booking.Id, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error releasing seats for booking {BookingId}", booking.Id);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}