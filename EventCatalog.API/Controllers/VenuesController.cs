using EventCatalog.Domain.Entities;
using EventCatalog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventCatalog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VenuesController : ControllerBase

{
    private readonly EventCatalogDbContext _context;
        
    public VenuesController(EventCatalogDbContext context)
    {
        _context = context;
    }

    // GET: api/venues
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Venue>>> GetAll() 
    {
        return Ok(await _context.Venues.ToListAsync());
    
    }
    // POST: api/venues
    [Authorize(Roles ="Admin")]
    [HttpPost]
    public async Task<ActionResult<Venue>> Create([FromBody] Venue venue)
    { 
         venue.Id = Guid.NewGuid();
         _context.Venues.Add(venue);
         await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = venue.Id }, venue);
        
    
    }



}
