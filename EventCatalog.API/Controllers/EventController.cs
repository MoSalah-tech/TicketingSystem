using EventCatalog.Application.Events;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace EventCatalog.API.Controllers;


[ApiController] 
[Route("api/[controller]")]

public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;
    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }
    // GET: api/events

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var events = await _eventService.GetAllAsync();
        return Ok(events);

    }

    // GET: api/events/{id}

    [HttpGet("{id}")]

    public async Task<IActionResult> GetById(Guid id)
    {

        var ev = await _eventService.GetByIdAsync(id);
        if (ev is null) return NotFound();
        return Ok(ev);

    }

    // POST: api/events
    [Authorize(Roles ="Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEventDto dto)
    {
        try
        {
            var created = await _eventService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);



        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });

        }




    }
}






