namespace Booking.API.Services;

public interface IBookingService
{
    Task<BookingDto> CreateBookingAsync(string userId, CreateBookingDto dto);
    Task<BookingDto?> GetByIdAsync(Guid id, string userId, bool isAdmin);
    Task<IEnumerable<BookingDto>> GetUserBookingsAsync(string userId);
    Task<bool> CancelBookingAsync(Guid id, string userId);
}