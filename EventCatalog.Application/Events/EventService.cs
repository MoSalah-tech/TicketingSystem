using EventCatalog.Domain.Entities;
using EventCatalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EventCatalog.Application.Events;

public class EventService : IEventService
{
    private readonly EventCatalogDbContext _context;

    public EventService(EventCatalogDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<EventDto>> GetAllAsync()
    {
        return await _context.Events
            .Select(e => new EventDto
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                EventDate = e.EventDate,
                VenueId = e.VenueId
            })
            .ToListAsync();
    }

    public async Task<EventDto?> GetByIdAsync(Guid id)
    {
        var e = await _context.Events.FindAsync(id);
        if (e is null) return null;

        return new EventDto
        {
            Id = e.Id,
            Name = e.Name,
            Description = e.Description,
            EventDate = e.EventDate,
            VenueId = e.VenueId
        };
    }

    public async Task<EventDto> CreateAsync(CreateEventDto dto)
    {
        // Business Rule: Check if the Venue actually exists before creating the Event!
        var venueExists = await _context.Venues.AnyAsync(v => v.Id == dto.VenueId);
        if (!venueExists)
        {
            throw new Exception("Venue does not exist.");
        }

        var newEvent = new Event
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            EventDate = dto.EventDate,
            VenueId = dto.VenueId
        };

        _context.Events.Add(newEvent);
        await _context.SaveChangesAsync();

        return new EventDto
        {
            Id = newEvent.Id,
            Name = newEvent.Name,
            Description = newEvent.Description,
            EventDate = newEvent.EventDate,
            VenueId = newEvent.VenueId
        };
    }
}