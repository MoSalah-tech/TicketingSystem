namespace EventCatalog.Application.Seats;

// What we send back to the client
public class SeatDto
{
    public Guid Id { get; set; }
    public string Row { get; set; } = string.Empty;
    public int Number { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
    public Guid EventId { get; set; }
}

// What the client sends us to generate seats in bulk
public class GenerateSeatsDto
{
    public Guid EventId { get; set; }
    public int NumberOfRows { get; set; }      // e.g., 10 rows
    public int SeatsPerRow { get; set; }       // e.g., 20 seats per row
    public decimal Price { get; set; }         // e.g., 50.00
}