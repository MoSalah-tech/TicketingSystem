namespace EventCatalog.Domain.Entities;

public class Seat
{
    public Guid Id { get; set; }
    public string Row { get; set; } = string.Empty;
    public int Number { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }


    // Foreign key to Event
    public Guid EventId { get; set; }
    public Event? Event { get; set; }



}
