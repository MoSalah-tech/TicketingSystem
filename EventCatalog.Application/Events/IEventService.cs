using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EventCatalog.Application.Events;

public interface IEventService
{
    Task<IEnumerable<EventDto>> GetAllAsync();
    Task<EventDto?> GetByIdAsync(Guid id);
    Task<EventDto> CreateAsync(CreateEventDto dto);
}