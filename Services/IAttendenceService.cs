using StarterKit.Models;

namespace StarterKit.Services
{
    public interface IAttendanceService
    {
        AttendanceResult AttendEvent(AttendanceModel model);
        List<User> GetAttendees(int eventId);
        AttendanceResult CancelAttendance(int eventId);
        AttendanceResult AddFeedback(int eventId, int rating, string feedback);
    }
}
