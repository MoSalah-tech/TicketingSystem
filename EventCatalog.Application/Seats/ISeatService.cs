namespace EventCatalog.Application.Seats;

public interface ISeatService
{
    Task<IEnumerable<SeatDto>> GetSeatsForEventAsync(Guid eventId);
    Task<IEnumerable<SeatDto>> GenerateSeatsAsync(GenerateSeatsDto dto);

    Task ReserveSeatsAsync(Guid eventId, List<Guid> seatIds);
    Task ReleaseSeatsAsync(Guid eventId, List<Guid> seatIds);

}