using Microsoft.EntityFrameworkCore;
using StarterKit.Models;
using StarterKit.Models.DTOs;
using StarterKit.Utils;
using System;

namespace StarterKit.Services
{
    public class EventService : IEventService
    {
        private readonly DatabaseContext _context;
        private readonly ILoginService _loginService;

        public EventService(DatabaseContext context, ILoginService loginService)
        {
            _context = context;
            _loginService = loginService;
        }

        public async Task<IEnumerable<EventDTO>> GetEventsAsync()
        {
            return await _context.Events
                .Include(e => e.Event_Attendances)
                    .ThenInclude(ea => ea.User)
                .Select(e => new EventDTO
                {
                    Id = e.EventId,
                    Title = e.Title,
                    Description = e.Description,
                    // Convert DateTime to DateOnly
                    Date = DateOnly.FromDateTime(e.EventDate),
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    Location = e.Location,
                    Reviews = _context.Reviews
                        .Where(r => r.EventId == e.EventId)
                        .Select(r => new ReviewDTO
                        {
                            Id = r.Id,
                            Comment = r.Comment,
                            Rating = r.Rating
                        }).ToList(),
                    Attendees = e.Event_Attendances.Select(ea => new AttendeeDTO
                    {
                        UserId = ea.User.UserId,
                        UserName = ea.User.FirstName + " " + ea.User.LastName
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<EventDTO> GetEventByIdAsync(int eventId)
        {
            var @event = await _context.Events
                .Include(e => e.Event_Attendances)
                    .ThenInclude(ea => ea.User)
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (@event == null)
                throw new EventNotFoundException(eventId);

            return new EventDTO
            {
                Id = @event.EventId,
                Title = @event.Title,
                Description = @event.Description,
                // Convert DateTime to DateOnly
                Date = DateOnly.FromDateTime(@event.EventDate),
                StartTime = @event.StartTime,
                EndTime = @event.EndTime,
                Location = @event.Location,
                Reviews = await _context.Reviews
                    .Where(r => r.EventId == eventId)
                    .Select(r => new ReviewDTO
                    {
                        Id = r.Id,
                        Comment = r.Comment,
                        Rating = r.Rating
                    }).ToListAsync(),
                Attendees = @event.Event_Attendances.Select(ea => new AttendeeDTO
                {
                    UserId = ea.User.UserId,
                    UserName = ea.User.FirstName + " " + ea.User.LastName
                }).ToList()
            };
        }

        public async Task<EventDTO> CreateEventAsync(EventCreateDTO eventCreateDTO)
        {
            var newEvent = new Event
            {
                Title = eventCreateDTO.Title,
                Description = eventCreateDTO.Description,
                EventDate = eventCreateDTO.Date,
                StartTime = eventCreateDTO.StartTime,
                EndTime = eventCreateDTO.EndTime,
                Location = eventCreateDTO.Location,
                Event_Attendances = new List<Event_Attendance>()
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return await GetEventByIdAsync(newEvent.EventId);
        }

        public async Task<EventDTO> UpdateEventAsync(int eventId, EventUpdateDTO eventUpdateDTO)
        {
            var existingEvent = await _context.Events.FindAsync(eventId);
            if (existingEvent == null)
                throw new EventNotFoundException(eventId);

            existingEvent.Title = eventUpdateDTO.Title;
            existingEvent.Description = eventUpdateDTO.Description;
            existingEvent.EventDate = eventUpdateDTO.Date;
            existingEvent.StartTime = eventUpdateDTO.StartTime;
            existingEvent.EndTime = eventUpdateDTO.EndTime;
            existingEvent.Location = eventUpdateDTO.Location;

            await _context.SaveChangesAsync();

            return await GetEventByIdAsync(eventId);
        }

        public async Task DeleteEventAsync(int eventId)
        {
            var @event = await _context.Events.FindAsync(eventId);
            if (@event == null)
                throw new EventNotFoundException(eventId);

            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
        }

        public async Task<ReviewDTO> CreateReviewAsync(int eventId, ReviewCreateDTO reviewCreateDTO)
        {
            // Validate event exists
            var @event = await _context.Events.FindAsync(eventId) 
                ?? throw new EventNotFoundException(eventId);

            // Check if user is logged in
            var user = _loginService.GetLoggedInUser() 
                ?? throw new UserNotAuthorizedException("User must be logged in to create a review");

            // Validate review
            if (reviewCreateDTO.Rating < 1 || reviewCreateDTO.Rating > 5)
                throw new InvalidReviewException("Rating must be between 1 and 5");

            // Check if user has attended the event
            var hasAttended = await _context.EventAttendances
                .AnyAsync(ea => ea.EventId == eventId && ea.User.UserId == user.UserId);

            if (!hasAttended)
                throw new EventAttendanceException("Only event attendees can leave a review");

            var review = new Review
            {
                EventId = eventId,
                UserId = user.UserId,
                Comment = reviewCreateDTO.Comment,
                Rating = reviewCreateDTO.Rating,
                CreatedDate = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return new ReviewDTO
            {
                Id = review.Id,
                Comment = review.Comment,
                Rating = review.Rating
            };
        }

        public async Task<EventDTO> AttendEventAsync(AttendEventDTO attendEventDto)
        {
            var user = _loginService.GetLoggedInUser()
                ?? throw new UserNotAuthorizedException("User must be logged in to attend an event");

            var @event = await _context.Events.FindAsync(attendEventDto.EventId)
                ?? throw new EventNotFoundException(attendEventDto.EventId);

            // Check event availability based on date and time
            if (@event.EventDate.Date < DateTime.Today.Date)
                throw new EventAttendanceException("Cannot attend past events");

            // Check if user is already attending
            var existingAttendance = await _context.EventAttendances
                .FirstOrDefaultAsync(ea => ea.EventId == attendEventDto.EventId && ea.User.UserId == user.UserId);

            if (existingAttendance != null)
                throw new EventAttendanceException("User is already attending this event");

            var eventAttendance = new Event_Attendance
            {
                EventId = attendEventDto.EventId,
                UserId = user.UserId,
                User = user,
                Event = @event,
                Feedback = ""  // Default empty feedback
            };

            _context.EventAttendances.Add(eventAttendance);
            await _context.SaveChangesAsync();

            return await GetEventByIdAsync(attendEventDto.EventId);
        }

        public async Task<IEnumerable<AttendeeDTO>> GetEventAttendeesAsync(int eventId)
        {
            // Validate event exists first
            _ = await _context.Events.FindAsync(eventId)
                ?? throw new EventNotFoundException(eventId);

            return await _context.EventAttendances
                .Where(ea => ea.EventId == eventId)
                .Select(ea => new AttendeeDTO
                {
                    UserId = ea.User.UserId,
                    UserName = ea.User.FirstName + " " + ea.User.LastName
                 }).ToListAsync();
        }

        public async Task RemoveEventAttendanceAsync(int eventId, int userId)
        {
            // Validate event exists
            var @event = await _context.Events.FindAsync(eventId)
                ?? throw new EventNotFoundException(eventId);

            // Validate user exists
            var user = await _context.Users.FindAsync(userId)
                ?? throw new UserNotAuthorizedException("User not found");

            // Check if the logged-in user is removing their own attendance or is an admin
            var loggedInUser = _loginService.GetLoggedInUser();
            var isAdmin = _loginService.IsAdminLoggedIn();

            if (loggedInUser?.UserId != userId && !isAdmin)
                throw new UserNotAuthorizedException("You are not authorized to remove this attendance");

            // Find the specific event attendance
            var eventAttendance = await _context.EventAttendances
                .FirstOrDefaultAsync(ea => ea.EventId == eventId && ea.UserId == userId);

            if (eventAttendance == null)
                throw new EventAttendanceException("User is not attending this event");

            // Remove the event attendance
            _context.EventAttendances.Remove(eventAttendance);
            await _context.SaveChangesAsync();
        }

    }
}