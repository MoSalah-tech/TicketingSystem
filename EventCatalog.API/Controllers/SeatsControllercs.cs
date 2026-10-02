using EventCatalog.Application.Seats;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventCatalog.API.Controllers;
[Authorize]
[ApiController]
[Route("api/events/{eventId}/[controller]")]
public class SeatsController : ControllerBase
{
    private readonly ISeatService _seatService;

    public SeatsController(ISeatService seatService)
    {
        _seatService = seatService;
    }

    // GET: api/events/{eventId}/seats
    [HttpGet]
    public async Task<IActionResult> GetSeatsForEvent(Guid eventId)
    {
        var seats = await _seatService.GetSeatsForEventAsync(eventId);
        return Ok(seats);
    }

    // POST: api/events/{eventId}/seats/generate
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateSeats(Guid eventId, [FromBody] GenerateSeatsDto dto)
    {
        try
        {
            // Make sure the eventId in the URL matches the one in the body
            if (dto.EventId != eventId)
                return BadRequest(new { error = "Event ID in URL does not match event ID in body." });

            var seats = await _seatService.GenerateSeatsAsync(dto);
            return Ok(seats);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }


    [HttpPost("reserve")]
    public async Task<IActionResult> ReserveSeats(Guid eventId, [FromBody] List<Guid> seatIds)
    {
        try
        {
            await _seatService.ReserveSeatsAsync(eventId, seatIds);
            return Ok(new { message = "Seats reserved successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("release")]
    public async Task<IActionResult> ReleaseSeats(Guid eventId, [FromBody] List<Guid> seatIds)
    {
        try
        {
            await _seatService.ReleaseSeatsAsync(eventId, seatIds);
            return Ok(new { message = "Seats released successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}