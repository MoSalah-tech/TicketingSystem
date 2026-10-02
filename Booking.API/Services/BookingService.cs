using System.Net.Http.Json;
using Booking.API.Data;
using Booking.API.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Booking.API.Services;

public class BookingService : IBookingService
{
    private readonly BookingDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BookingService(
        BookingDbContext context,
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    private void AttachUserToken()
    {
        var authHeader = _httpContextAccessor.HttpContext?
            .Request.Headers["Authorization"].ToString();

        if (!string.IsNullOrEmpty(authHeader))
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(
                "Authorization", authHeader);
        }
    }

    public async Task<BookingDto> CreateBookingAsync(string userId, CreateBookingDto dto)
    {
        AttachUserToken();   // 👈 forward JWT to EventCatalog

        if (dto.SeatIds is null || dto.SeatIds.Count == 0)
            throw new Exception("At least one seat must be selected.");

        var reserveResponse = await _httpClient.PostAsJsonAsync(
            $"/api/events/{dto.EventId}/seats/reserve",
            dto.SeatIds);

        if (!reserveResponse.IsSuccessStatusCode)
        {
            var error = await reserveResponse.Content.ReadAsStringAsync();
            throw new Exception($"Failed to reserve seats: {reserveResponse.StatusCode} - {error}");
        }

        var allSeats = await _httpClient.GetFromJsonAsync<List<SeatDto>>(
            $"/api/events/{dto.EventId}/seats");

        var selectedSeats = allSeats?
            .Where(s => dto.SeatIds.Contains(s.Id))
            .ToList() ?? new List<SeatDto>();

        var booking = new BookingEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            EventId = dto.EventId,
            Status = BookingStatus.Pending,
            TotalPrice = selectedSeats.Sum(s => s.Price),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10)
        };

        foreach (var seat in selectedSeats)
        {
            booking.Seats.Add(new BookingSeat
            {
                Id = Guid.NewGuid(),
                BookingId = booking.Id,
                SeatId = seat.Id,
                Price = seat.Price
            });
        }

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return MapToDto(booking);
    }

    public async Task<BookingDto?> GetByIdAsync(Guid id, string userId, bool isAdmin)
    {
        var booking = await _context.Bookings
            .Include(b => b.Seats)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking is null) return null;
        if (!isAdmin && booking.UserId != userId) return null;

        return MapToDto(booking);
    }

    public async Task<IEnumerable<BookingDto>> GetUserBookingsAsync(string userId)
    {
        var bookings = await _context.Bookings
            .Include(b => b.Seats)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return bookings.Select(MapToDto);
    }

    public async Task<bool> CancelBookingAsync(Guid id, string userId)
    {
        AttachUserToken();   // 👈 also forward on cancel

        var booking = await _context.Bookings
            .Include(b => b.Seats)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking is null) return false;
        if (booking.UserId != userId)
            throw new Exception("You can only cancel your own bookings.");
        if (booking.Status != BookingStatus.Pending)
            throw new Exception("Only pending bookings can be cancelled.");

        var seatIds = booking.Seats.Select(s => s.SeatId).ToList();
        await _httpClient.PostAsJsonAsync(
            $"/api/events/{booking.EventId}/seats/release",
            seatIds);

        booking.Status = BookingStatus.Cancelled;
        await _context.SaveChangesAsync();

        return true;
    }

    private static BookingDto MapToDto(BookingEntity b)
    {
        return new BookingDto
        {
            Id = b.Id,
            UserId = b.UserId,
            EventId = b.EventId,
            Status = b.Status.ToString(),
            TotalPrice = b.TotalPrice,
            CreatedAt = b.CreatedAt,
            ExpiresAt = b.ExpiresAt,
            SeatIds = b.Seats.Select(s => s.SeatId).ToList()
        };
    }
}