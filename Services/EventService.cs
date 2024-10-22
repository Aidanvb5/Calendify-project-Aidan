using StarterKit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace StarterKit.Services;

  public class EventService : IEventService
{
    private readonly DatabaseContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EventService(DatabaseContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public List<Event> GetEvents()
    {
        return _context.Event
            .Include(e => e.Event_Attendances)
                .ThenInclude(ea => ea.User)
            .OrderBy(e => e.EventDate)
            .ThenBy(e => e.StartTime)
            .ToList();
    }

    public EventResult CreateEvent(EventModel model)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(model.Title) || 
            string.IsNullOrWhiteSpace(model.Description) ||
            string.IsNullOrWhiteSpace(model.Location))
        {
            return new EventResult 
            { 
                Success = false, 
                Message = "All fields are required" 
            };
        }

        // Validate date and time
        if (model.Date < DateTime.Today)
        {
            return new EventResult 
            { 
                Success = false, 
                Message = "Event date cannot be in the past" 
            };
        }

        if (model.EndTime <= model.StartTime)
        {
            return new EventResult 
            { 
                Success = false, 
                Message = "End time must be after start time" 
            };
        }

        var eventEntity = new Event
        {
            Title = model.Title,
            Description = model.Description,
            EventDate = model.Date,
            StartTime = model.StartTime,
            EndTime = model.EndTime,
            Location = model.Location,
            AdminApproval = false,
            Event_Attendances = new List<Event_Attendance>()
        };

        try
        {
            _context.Event.Add(eventEntity);
            _context.SaveChanges();

            return new EventResult 
            { 
                Success = true,
                Message = "Event created successfully" 
            };
        }
        catch (Exception ex)
        {
            return new EventResult 
            { 
                Success = false,
                Message = "An error occurred while creating the event" 
            };
        }
    }

    public EventResult UpdateEvent(int eventId, EventModel model)
    {
        var eventEntity = _context.Event.Find(eventId);
        if (eventEntity == null)
        {
            return new EventResult 
            { 
                Success = false, 
                Message = "Event not found" 
            };
        }

        // Validate input
        if (string.IsNullOrWhiteSpace(model.Title) || 
            string.IsNullOrWhiteSpace(model.Description) ||
            string.IsNullOrWhiteSpace(model.Location))
        {
            return new EventResult 
            { 
                Success = false, 
                Message = "All fields are required" 
            };
        }

        // Validate date and time
        if (model.Date < DateTime.Today)
        {
            return new EventResult 
            { 
                Success = false, 
                Message = "Event date cannot be in the past" 
            };
        }

        if (model.EndTime <= model.StartTime)
        {
            return new EventResult 
            { 
                Success = false, 
                Message = "End time must be after start time" 
            };
        }

        try
        {
            eventEntity.Title = model.Title;
            eventEntity.Description = model.Description;
            eventEntity.EventDate = model.Date;
            eventEntity.StartTime = model.StartTime;
            eventEntity.EndTime = model.EndTime;
            eventEntity.Location = model.Location;

            _context.SaveChanges();

        return new EventResult 
            { 
                Success = true,
                Message = "Event updated successfully"
            };
        }
        catch (Exception ex)
        {
            return new EventResult 
            { 
                Success = false,
                Message = "An error occurred while updating the event" 
            };
        }
    }

    public EventResult DeleteEvent(int eventId)
    {
        var eventEntity = _context.Event
            .Include(e => e.Event_Attendances)
            .FirstOrDefault(e => e.EventId == eventId);

        if (eventEntity == null)
        {
            return new EventResult 
            { 
                Success = false, 
                Message = "Event not found" 
            };
        }

        try
        {
            // First remove all attendances
            _context.Event_Attendance.RemoveRange(eventEntity.Event_Attendances);
            
            // Then remove the event
            _context.Event.Remove(eventEntity);
            _context.SaveChanges();

            return new EventResult 
            { 
                Success = true,
                Message = "Event deleted successfully" 
            };
        }
        catch (Exception ex)
        {
            return new EventResult 
            { 
                Success = false,
                Message = "An error occurred while deleting the event" 
            };
        }
    }

    // Additional helper methods
    private bool IsUserAdmin()
    {
        var userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");
        return userRole == "Admin";
    }

    private int? GetCurrentUserId()
    {
        var userIdString = _httpContextAccessor.HttpContext?.Session.GetString("UserId");
        if (int.TryParse(userIdString, out int userId))
        {
            return userId;
        }
        return null;
    }
}