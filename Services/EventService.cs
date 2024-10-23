using StarterKit.Models;
using StarterKit.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StarterKit.Services
{
    public class EventService : IEventService
    {
        private readonly DatabaseContext _context;

        public EventService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EventDTO>> GetEventsAsync()
        {
            var events = await _context.Events
                .Include(e => e.Reviews)
                .Include(e => e.Attendees)
                .ToListAsync();

            return events.Select(e => new EventDTO
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Date = e.Date,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                Location = e.Location,
                Reviews = e.Reviews.Select(r => new ReviewDTO
                {
                    Id = r.Id,
                    EventId = r.EventId,
                    UserId = r.UserId,
                    Rating = r.Rating,
                    Comment = r.Comment
                }),
                Attendees = e.Attendees.Select(a => new AttendeeDTO
                {
                    Id = a.Id,
                    EventId = a.EventId,
                    UserId = a.UserId
                })
            });
        }

        public async Task<EventDTO> CreateEventAsync(EventCreateDTO eventCreateDTO)
        {
            var @event = new Event
            {
                Title = eventCreateDTO.Title,
                Description = eventCreateDTO.Description,
                Date = eventCreateDTO.Date,
                StartTime = eventCreateDTO.StartTime,
                EndTime = eventCreateDTO.EndTime,
                Location = eventCreateDTO.Location
            };

            _context.Events.Add(@event);
            await _context.SaveChangesAsync();

            return new EventDTO
            {
                Id = @event.Id,
                Title = @event.Title,
                Description = @event.Description,
                Date = @event.Date,
                StartTime = @event.StartTime,
                EndTime = @event.EndTime,
                Location = @event.Location
            };
        }

        public async Task<EventDTO> UpdateEventAsync(int id, EventUpdateDTO eventUpdateDTO)
        {
            var @event = await _context.Events.FindAsync(id);

            if (@event == null)
            {
                throw new NotFoundException("Event not found");
            }

            @event.Title = eventUpdateDTO.Title;
            @event.Description = eventUpdateDTO.Description;
            @event.Date = eventUpdateDTO.Date;
            @event.StartTime = eventUpdateDTO.StartTime;
            @event.EndTime = eventUpdateDTO.EndTime;
            @event.Location = eventUpdateDTO.Location;

            await _context.SaveChangesAsync();

            return new EventDTO
            {
                Id = @event.Id,
                Title = @event.Title,
                Description = @event.Description,
                Date = @event.Date,
                StartTime = @event.StartTime,
                EndTime = @event.EndTime,
                Location = @event.Location
            };
        }

        public async Task DeleteEventAsync(int id)
        {
            var @event = await _context.Events.FindAsync(id);

            if (@event == null)
            {
                throw new NotFoundException("Event not found");
            }

            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
        }

        public async Task<ReviewDTO> CreateReviewAsync(int eventId, ReviewCreateDTO reviewCreateDTO)
        {
            var review = new Review
            {
                EventId = eventId,
                UserId = reviewCreateDTO.UserId,
                Rating = reviewCreateDTO.Rating,
                Comment = reviewCreateDTO.Comment
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return new ReviewDTO
            {
                Id = review.Id,
                EventId = review.EventId,
                UserId = review.UserId,
                Rating = review.Rating,
                Comment = review.Comment
            };
        }
    }
}