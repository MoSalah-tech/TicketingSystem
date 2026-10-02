using Booking.API.Data;
using Booking.API.Domain;
using Booking.API.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using System.Net;

namespace Booking.API.Tests.Services;

public class BookingServiceTests : IDisposable
{
    private readonly BookingDbContext _context;
    private readonly Mock<HttpClient> _httpClientMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;

    public BookingServiceTests()
    {
        // Use an in-memory database so each test runs isolated
        var options = new DbContextOptionsBuilder<BookingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new BookingDbContext(options);

        _httpClientMock = new Mock<HttpClient>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
    }

    private BookingService CreateSut()
        => new BookingService(_context, _httpClientMock.Object, _httpContextAccessorMock.Object);

    public void Dispose() => _context.Dispose();

    // --- TEST 1 ---
    [Fact]
    public async Task CreateBooking_WithNoSeats_ThrowsException()
    {
        // Arrange
        var sut = CreateSut();
        var dto = new CreateBookingDto
        {
            EventId = Guid.NewGuid(),
            SeatIds = new List<Guid>()   // 👈 empty!
        };

        // Act
        var act = async () => await sut.CreateBookingAsync("user-1", dto);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("At least one seat must be selected.");
    }

    // --- TEST 2 ---
    [Fact]
    public async Task CancelBooking_WhenUserDoesNotOwnBooking_ThrowsException()
    {
        // Arrange: create a booking owned by "user-1"
        var booking = new BookingEntity
        {
            Id = Guid.NewGuid(),
            UserId = "user-1",
            EventId = Guid.NewGuid(),
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10)
        };
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        var sut = CreateSut();

        // Act: try to cancel as "user-2"
        var act = async () => await sut.CancelBookingAsync(booking.Id, "user-2");

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("You can only cancel your own bookings.");
    }

    // --- TEST 3 ---
    [Fact]
    public async Task CancelBooking_WhenAlreadyCancelled_ThrowsException()
    {
        // Arrange
        var booking = new BookingEntity
        {
            Id = Guid.NewGuid(),
            UserId = "user-1",
            EventId = Guid.NewGuid(),
            Status = BookingStatus.Cancelled,   // 👈 already cancelled
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10)
        };
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        var sut = CreateSut();

        // Act
        var act = async () => await sut.CancelBookingAsync(booking.Id, "user-1");

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Only pending bookings can be cancelled.");
    }

    // --- TEST 4 ---
    [Fact]
    public async Task GetById_WhenUserIsNotOwnerAndNotAdmin_ReturnsNull()
    {
        // Arrange
        var booking = new BookingEntity
        {
            Id = Guid.NewGuid(),
            UserId = "user-1",
            EventId = Guid.NewGuid(),
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10)
        };
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        var sut = CreateSut();

        // Act: ask as "user-2", not admin
        var result = await sut.GetByIdAsync(booking.Id, "user-2", isAdmin: false);

        // Assert
        result.Should().BeNull();
    }

    // --- TEST 5 ---
    [Fact]
    public async Task GetById_WhenUserIsAdmin_ReturnsBooking()
    {
        // Arrange
        var booking = new BookingEntity
        {
            Id = Guid.NewGuid(),
            UserId = "user-1",
            EventId = Guid.NewGuid(),
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10)
        };
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        var sut = CreateSut();

        // Act: ask as admin
        var result = await sut.GetByIdAsync(booking.Id, "user-2", isAdmin: true);

        // Assert
        result.Should().NotBeNull();
        result!.UserId.Should().Be("user-1");
    }
}