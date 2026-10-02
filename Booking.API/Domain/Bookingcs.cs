namespace Booking.API.Domain;

public class BookingEntity
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid EventId { get; set; }
    public BookingStatus Status { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public ICollection<BookingSeat> Seats { get; set; } = new List<BookingSeat>();

}

public enum BookingStatus
{
    Pending = 0,
    Confirmed = 1,
    Cancelled = 2,
    Expired = 3
}

public class BookingSeat
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public BookingEntity? Booking { get; set; }
    public Guid SeatId { get; set; }
    public decimal Price { get; set; }
}

