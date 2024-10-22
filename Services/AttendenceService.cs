using Microsoft.EntityFrameworkCore;
using StarterKit.Models;

namespace StarterKit.Services;

public class AttendanceService : IAttendanceService
{
    private readonly DatabaseContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    // Constructor should ensure both fields are initialized
    public AttendanceService(DatabaseContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public AttendanceResult AttendEvent(AttendanceModel model)
    {
        var eventEntity = _context.Events.Find(model.EventId);
        
        if (eventEntity == null)
        {
            return new AttendanceResult { Success = false, Message = "Event not found" };
        }

        // Check if the event has already ended
        if (eventEntity.EventDate < DateOnly.FromDateTime(DateTime.Now))
        {
            return new AttendanceResult { Success = false, Message = "Event has already ended" };
        }

        // Get the current user
        var userId = GetCurrentUser Id(); // Corrected method name
        if (!userId.HasValue)
        {
            return new AttendanceResult { Success = false, Message = "User  not authenticated" };
        }

        // Proceed with the attendance logic
        var attendance = new Attendance
        {
            UserId = userId.Value, // Set the UserId
            EventId = model.EventId,
            AttendanceDate = DateTime.Now,
            User = _context.Users.Find(userId.Value), // Set the User property
            Event = eventEntity // Set the Event property
        };

        _context.Attendances.Add(attendance);
        _context.SaveChanges();

        return new AttendanceResult { Success = true, Message = "Attendance recorded successfully" };
    }

    public List<User> GetAttendees(int eventId)
    {
        return _context.EventAttendances
            .Where(a => a.EventId == eventId)
            .Include(a => a.User)
            .Select(a => a.User)
            .ToList();
    }

    public AttendanceResult CancelAttendance(int eventId)
    {
        var userId = GetCurrentUser Id(); // Correct method name
        if (!userId.HasValue)
        {
            return new AttendanceResult 
            { 
                Success = false, 
                Message = "User  not authenticated" 
            };
        }

        var attendance = _context.EventAttendances
            .FirstOrDefault(a => a.EventId == eventId && a.UserId == userId.Value);

        if (attendance == null)
        {
            return new AttendanceResult 
            { 
                Success = false, 
                Message = "Attendance record not found" 
            };
        }

        // Check if event hasn't started yet
        var eventEntity = _context.Events.Find(eventId);
        if (eventEntity == null)
        {
            return new AttendanceResult 
            { 
                Success = false, 
                Message = "Event not found" 
            };
        }

        if (eventEntity.EventDate < DateOnly.FromDateTime(DateTime.Now))
        {
            return new AttendanceResult 
            { 
                Success = false, 
                Message = "Cannot cancel attendance after event has started" 
            };
        }

        try
        {
            _context.EventAttendances.Remove(attendance);
            _context.SaveChanges();

            return new AttendanceResult 
            { 
                Success = true,
                Message = "Attendance cancelled successfully" 
            };
        }
        catch (Exception ex)
        {
            return new AttendanceResult 
            { 
                Success = false,
                Message = "An error occurred while cancelling attendance: " + ex.Message 
            };
        }
    }

    public List<Attendance> GetUserAttendances(int userId)
    {
        throw new NotImplementedException();
    }

    private int? GetCurrentUser Id() // Corrected method name and added return type
    {
        var userIdString = _httpContextAccessor.HttpContext?.Session.GetString("User Id");
        if (int.TryParse(userIdString, out int userId))
        {
            return userId;
        }
        return null;
    }
}