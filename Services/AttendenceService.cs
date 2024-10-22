using StarterKit.Models;

namespace StarterKit.Services;

public class AttendanceService : IAttendanceService
{
    private readonly DatabaseContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AttendanceService(DatabaseContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public AttendanceResult AttendEvent(AttendanceModel model)
    {
        // Validate user
        var user = _context.User.Find(model.UserId);
        if (user == null)
        {
            return new AttendanceResult { Success = false, Message = "User not found" };
        }

        // Validate event
        var eventEntity = _context.Event.Find(model.EventId);
        if (eventEntity == null)
        {
            return new AttendanceResult { Success = false, Message = "Event not found" };
        }

        // Check if user is already attending
        var existingAttendance = _context.Event_Attendance
            .FirstOrDefault(a => a.UserId == model.UserId && a.EventId == model.EventId);

        if (existingAttendance != null)
        {
            return new AttendanceResult { Success = false, Message = "Already attending this event" };
        }

        // Check if event hasn't started yet
        if (eventEntity.EventDate < DateTime.Today || 
            (eventEntity.EventDate == DateTime.Today && eventEntity.StartTime < DateTime.Now.TimeOfDay))
        {
            return new AttendanceResult { Success = false, Message = "Event has already started or ended" };
        }

        try
        {
            var attendance = new Event_Attendance
            {
                UserId = model.UserId,
                EventId = model.EventId,
                Rating = 0, // Default rating
                Feedback = "" // Empty feedback initially
            };

            _context.Event_Attendance.Add(attendance);
            _context.SaveChanges();

            return new AttendanceResult 
            { 
                Success = true,
                Message = "Successfully registered for the event"
            };
        }
        catch (Exception ex)
        {
            return new AttendanceResult 
            { 
                Success = false,
                Message = "An error occurred while registering for the event"
            };
        }
    }

    public List<User> GetAttendees(int eventId)
    {
        return _context.Event_Attendance
            .Where(a => a.EventId == eventId)
            .Include(a => a.User)
            .Select(a => a.User)
            .ToList();
    }

    public AttendanceResult CancelAttendance(int eventId)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return new AttendanceResult 
            { 
                Success = false, 
                Message = "User not authenticated" 
            };
        }

        var attendance = _context.Event_Attendance
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
        var eventEntity = _context.Event.Find(eventId);
        if (eventEntity == null)
        {
            return new AttendanceResult 
            { 
                Success = false, 
                Message = "Event not found" 
            };
        }

        if (eventEntity.EventDate < DateTime.Today || 
            (eventEntity.EventDate == DateTime.Today && eventEntity.StartTime < DateTime.Now.TimeOfDay))
        {
            return new AttendanceResult 
            { 
                Success = false, 
                Message = "Cannot cancel attendance after event has started" 
            };
        }

        try
        {
            _context.Event_Attendance.Remove(attendance);
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
                Message = "An error occurred while cancelling attendance" 
            };
        }
    }

    public AttendanceResult AddFeedback(int eventId, int rating, string feedback)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return new AttendanceResult 
            { 
                Success = false, 
                Message = "User not authenticated" 
            };
        }

        var attendance = _context.Event_Attendance
            .FirstOrDefault(a => a.EventId == eventId && a.UserId == userId.Value);

        if (attendance == null)
        {
            return new AttendanceResult 
            { 
                Success = false, 
                Message = "Attendance record not found" 
            };
        }

        // Validate rating
        if (rating < 1 || rating > 5)
        {
            return new AttendanceResult 
            { 
                Success = false, 
                Message = "Rating must be between 1 and 5" 
            };
        }

        try
        {
            attendance.Rating = rating;
            attendance.Feedback = feedback;
            _context.SaveChanges();

            return new AttendanceResult 
            { 
                Success = true,
                Message = "Feedback added successfully" 
            };
        }
        catch (Exception ex)
        {
            return new AttendanceResult 
            { 
                Success = false,
                Message = "An error occurred while adding feedback" 
            };
        }
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