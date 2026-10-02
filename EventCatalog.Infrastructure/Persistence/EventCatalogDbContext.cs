using EventCatalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventCatalog.Infrastructure.Persistence;

public class EventCatalogDbContext : DbContext
{
    public EventCatalogDbContext(DbContextOptions<EventCatalogDbContext> options)
        : base(options) { }

    public DbSet<Event> Events => Set<Event>();
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<Seat> Seats => Set<Seat>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure relationships and constraints
        modelBuilder.Entity<Event>()
            .HasOne(e => e.Venue)
            .WithMany(v => v.Events)
            .HasForeignKey(e => e.VenueId);

        modelBuilder.Entity<Seat>()
            .HasOne(s => s.Event)
            .WithMany(e => e.Seats)
            .HasForeignKey(s => s.EventId);

        modelBuilder.Entity<Seat>()
            .Property(s => s.Price)
            .HasPrecision(18, 2);
    }
}