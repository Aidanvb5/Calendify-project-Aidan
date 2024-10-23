using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using StarterKit.Services;
using StarterKit.Models;
using StarterKit.Models.DTOs;

namespace StarterKit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly ILoginService _loginService;

        public EventsController(IEventService eventService, ILoginService loginService)
        {
            _eventService = eventService;
            _loginService = loginService;
        }

        // GET: api/events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventDTO>>> GetEvents()
        {
            var events = await _eventService.GetEventsAsync();
            return Ok(events);
        }

        // POST: api/events
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<EventDTO>> CreateEvent(EventCreateDTO eventCreateDTO)
        {
            var @event = await _eventService.CreateEventAsync(eventCreateDTO);
            return CreatedAtAction(nameof(GetEvents), new { id = @event.Id }, @event);
        }

        // PUT: api/events/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<EventDTO>> UpdateEvent(int id, EventUpdateDTO eventUpdateDTO)
        {
            var @event = await _eventService.UpdateEventAsync(id, eventUpdateDTO);
            return Ok(@event);
        }

        // DELETE: api/events/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEvent(int id)
        {
            await _eventService.DeleteEventAsync(id);
            return NoContent();
        }

        // POST: api/events/5/reviews
        [Authorize]
        [HttpPost("{eventId}/reviews")]
        public async Task<ActionResult<ReviewDTO>> CreateReview(int eventId, ReviewCreateDTO reviewCreateDTO)
        {
            var review = await _eventService.CreateReviewAsync(eventId, reviewCreateDTO);
            return CreatedAtAction(nameof(GetEvents), new { id = review.Id }, review);
        }
    }
}