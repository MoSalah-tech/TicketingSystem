namespace EventCatalog.Application.Events;

// What we send BACK to the user
public class EventDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public Guid VenueId { get; set; }
}
// What the user SENDS to us to create an event
public class CreateEventDto 
{

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public Guid VenueId { get; set; }


}
