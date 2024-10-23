using Microsoft.AspNetCore.Mvc;
using StarterKit.Models;
using StarterKit.Models.DTOs;
using StarterKit.Services;

namespace StarterKit.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class EventController : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly ILoginService _loginService;

    public EventController(DatabaseContext context, ILoginService loginService)
    {
        _context = context;
        _loginService = loginService;
    }

    // GET: api/v1/Event
    [HttpGet]
    public IActionResult GetEvents()
    {
        var events = _context.Event
            .Include(e => e.Event_Attendances)
                .ThenInclude(ea => ea.User)
            .ToList();
        return Ok(events);
    }

    // GET: api/v1/Event/5
    [HttpGet("{id}")]
    public IActionResult GetEvent(int id)
    {
        var @event = _context.Event
            .Include(e => e.Event_Attendances)
                .ThenInclude(ea => ea.User)
            .FirstOrDefault(e => e.EventId == id);

        if (@event == null)
        {
            return NotFound();
        }

        return Ok(@event);
    }

    // POST: api/v1/Event
    [HttpPost]
    public IActionResult CreateEvent([FromBody] EventCreateDTO eventDto)
    {
        if (!IsAdminLoggedIn())
        {
            return Unauthorized("Admin login required");
        }

        var newEvent = new Event
        {
            Title = eventDto.Title,
            Description = eventDto.Description,
            EventDate = eventDto.EventDate,
            StartTime = eventDto.StartTime,
            EndTime = eventDto.EndTime,
            Location = eventDto.Location,
            AdminApproval = true,
            Event_Attendances = new List<Event_Attendance>()
        };

        _context.Event.Add(newEvent);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetEvent), new { id = newEvent.EventId }, newEvent);
    }

    // PUT: api/v1/Event/5
    [HttpPut("{id}")]
    public IActionResult UpdateEvent(int id, [FromBody] EventCreateDTO eventDto)
    {
        if (!IsAdminLoggedIn())
        {
            return Unauthorized("Admin login required");
        }

        var @event = _context.Event.Find(id);
        if (@event == null)
        {
            return NotFound();
        }

        @event.Title = eventDto.Title;
        @event.Description = eventDto.Description;
        @event.EventDate = eventDto.EventDate;
        @event.StartTime = eventDto.StartTime;
        @event.EndTime = eventDto.EndTime;
        @event.Location = eventDto.Location;

        _context.SaveChanges();

        return Ok(@event);
    }

    // DELETE: api/v1/Event/5
    [HttpDelete("{id}")]
    public IActionResult DeleteEvent(int id)
    {
        if (!IsAdminLoggedIn())
        {
            return Unauthorized("Admin login required");
        }

        var @event = _context.Event.Find(id);
        if (@event == null)
        {
            return NotFound();
        }

        _context.Event.Remove(@event);
        _context.SaveChanges();

        return NoContent();
    }

    // POST: api/v1/Event/5/review
    [HttpPost("{id}/review")]
    public IActionResult AddReview(int id, [FromBody] ReviewDTO reviewDto)
    {
        var @event = _context.Event.Find(id);
        if (@event == null)
        {
            return NotFound();
        }

        var newReview = new Review
        {
            EventId = id,
            Text = reviewDto.Text,
            Rating = reviewDto.Rating
        };

        _context.Review.Add(newReview);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetEvent), new { id = id }, newReview);
    }

    private bool IsAdminLoggedIn()
    {
        return _loginService.IsAdminLoggedIn();
    }
}