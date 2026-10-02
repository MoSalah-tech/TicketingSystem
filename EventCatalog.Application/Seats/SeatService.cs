using EventCatalog.Domain.Entities;
using EventCatalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventCatalog.Application.Seats;

public class SeatService : ISeatService
{
    private readonly EventCatalogDbContext _context;

    public SeatService(EventCatalogDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SeatDto>> GetSeatsForEventAsync(Guid eventId)
    {
        return await _context.Seats
            .Where(s => s.EventId == eventId)
            .OrderBy(s => s.Row)
            .ThenBy(s => s.Number)
            .Select(s => new SeatDto
            {
                Id = s.Id,
                Row = s.Row,
                Number = s.Number,
                Price = s.Price,
                IsAvailable = s.IsAvailable,
                EventId = s.EventId
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<SeatDto>> GenerateSeatsAsync(GenerateSeatsDto dto)
    {
        // Business Rule 1: Does the event exist?
        var eventExists = await _context.Events.AnyAsync(e => e.Id == dto.EventId);
        if (!eventExists)
            throw new Exception("Event does not exist.");

        // Business Rule 2: Prevent duplicate seat generation
        var alreadyHasSeats = await _context.Seats.AnyAsync(s => s.EventId == dto.EventId);
        if (alreadyHasSeats)
            throw new Exception("Seats have already been generated for this event.");

        // Business Rule 3: Sanity checks
        if (dto.NumberOfRows <= 0 || dto.SeatsPerRow <= 0)
            throw new Exception("Number of rows and seats per row must be greater than zero.");

        if (dto.NumberOfRows > 26)
            throw new Exception("Maximum of 26 rows allowed (A-Z).");

        var seats = new List<Seat>();
        var rowLabels = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        for (int r = 0; r < dto.NumberOfRows; r++)
        {
            for (int n = 1; n <= dto.SeatsPerRow; n++)
            {
                seats.Add(new Seat
                {
                    Id = Guid.NewGuid(),
                    Row = rowLabels[r].ToString(),
                    Number = n,
                    Price = dto.Price,
                    IsAvailable = true,
                    EventId = dto.EventId
                });
            }
        }

        _context.Seats.AddRange(seats);
        await _context.SaveChangesAsync();

        return seats.Select(s => new SeatDto
        {
            Id = s.Id,
            Row = s.Row,
            Number = s.Number,
            Price = s.Price,
            IsAvailable = s.IsAvailable,
            EventId = s.EventId
        });
    }

    public async Task ReserveSeatsAsync(Guid eventId, List<Guid> seatIds) 
    {
        var seats = await _context.Seats
        .Where(s => s.EventId == eventId && seatIds.Contains(s.Id))
        .ToListAsync();

        if (seats.Count != seatIds.Count)
            throw new Exception("One or more seats do not exist.");

        if (seats.Any(s => !s.IsAvailable))
            throw new Exception("One or more seats are already taken.");

        foreach (var seat in seats)
            seat.IsAvailable = false;

        await _context.SaveChangesAsync();



    }


    public async Task ReleaseSeatsAsync(Guid eventId, List<Guid> seatIds)
    {
        var seats = await _context.Seats
            .Where(s => s.EventId == eventId && seatIds.Contains(s.Id))
            .ToListAsync();

        foreach (var seat in seats)
            seat.IsAvailable = true;

        await _context.SaveChangesAsync();
    }
}