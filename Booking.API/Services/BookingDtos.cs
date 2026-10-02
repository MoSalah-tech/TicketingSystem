namespace Booking.API.Services;

public class CreateBookingDto
{
    
    public Guid EventId { get; set; }
    public List<Guid> SeatIds { get; set; } = new();
}

public class BookingDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid EventId { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public List<Guid> SeatIds { get; set; } = new();
}

// Used to deserialize the seat data from Event Catalog
public class SeatDto
{
    public Guid Id { get; set; }
    public string Row { get; set; } = string.Empty;
    public int Number { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
    public Guid EventId { get; set; }
}