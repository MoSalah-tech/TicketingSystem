using Booking.API.Domain;
using Microsoft.EntityFrameworkCore;

namespace Booking.API.Data;

public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

    public DbSet<BookingEntity> Bookings => Set<BookingEntity>();
    public DbSet<BookingSeat> BookingSeats => Set<BookingSeat>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookingSeat>()
            .Property(s => s.Price)
            .HasPrecision(18, 2);
        modelBuilder.Entity<BookingEntity>()
            .Property(b => b.TotalPrice)
            .HasPrecision(18, 2);

    }
}
