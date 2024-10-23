using StarterKit.Models;
using StarterKit.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StarterKit.Utils;

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
                .Include(e => e.Event_Attendances) // Include Event_Attendances instead of Reviews
                .Include(e => e.Event_Attendances.Select(ea => ea.User)) // Optionally include User if needed
                .ToListAsync();

            return events.Select(e => new EventDTO
            {
                Id = e.EventId,
                Title = e.Title,
                Description = e.Description,
                Date = e.EventDate,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                Location = e.Location,
                Reviews = e.Event_Attendances.Select(ea => new ReviewDTO // Map Feedback to ReviewDTO
                {
                    // Assuming ReviewDTO has properties similar to Feedback
                    // Modify the properties based on your actual ReviewDTO structure
                    Id = ea.Event_AttendanceId, // Use Event_AttendanceId as the Id
                    EventId = e.EventId,
                    UserId = ea.User.UserId, // Assuming User is included
                    Rating = ea.Rating,
                    Comment = ea.Feedback // Use Feedback for the Comment
                }),
                Attendees = e.Event_Attendances.Select(a => new AttendeeDTO
                {
                    Id = a.Event_AttendanceId,
                    EventId = a.Event.EventId,
                    UserId = a.User.UserId
                })
            });
        }

        public async Task<EventDTO> CreateEventAsync(EventCreateDTO eventCreateDTO)
        {
            var @event = new Event
            {
                Title = eventCreateDTO.Title,
                Description = eventCreateDTO.Description,
                EventDate = eventCreateDTO.Date, // Make sure to use EventDate instead of Date
                StartTime = eventCreateDTO.StartTime,
                EndTime = eventCreateDTO.EndTime,
                Location = eventCreateDTO.Location,
                Event_Attendances = new List<Event_Attendance>() // Initialize Event_Attendances
            };

            _context.Events.Add(@event);
            await _context.SaveChangesAsync();

            return new EventDTO
            {
                Id = @event.EventId, // Make sure to use EventId instead of Id
                Title = @event.Title,
                Description = @event.Description,
                Date = @event.EventDate, // Make sure to use EventDate instead of Date
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
            @event.EventDate = eventUpdateDTO.Date;
            @event.StartTime = eventUpdateDTO.StartTime;
            @event.EndTime = eventUpdateDTO.EndTime;
            @event.Location = eventUpdateDTO.Location;

            await _context.SaveChangesAsync();

            return new EventDTO
            {
                Id = @event.EventId,
                Title = @event.Title,
                Description = @event.Description,
                Date = @event.EventDate,
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
                UserId = reviewCreateDTO.UserId, // Ensure reviewCreateDTO has UserId property
                Rating = reviewCreateDTO.Rating,   // Ensure reviewCreateDTO has Rating property
                Comment = reviewCreateDTO.Comment    // Ensure reviewCreateDTO has Comment property
            };

            _context.Reviews.Add(review); // Ensure the context is using Reviews
            await _context.SaveChangesAsync();

            return new ReviewDTO
            {
                Id = review.Id,                // Assuming Id is the primary key for Review
                EventId = review.EventId,
                UserId = review.UserId,
                Rating = review.Rating,
                Comment = review.Comment
            };
        }
    }
}