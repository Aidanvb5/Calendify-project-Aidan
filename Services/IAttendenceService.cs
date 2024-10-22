using StarterKit.Models;

namespace StarterKit.Services
{
    public interface IAttendanceService
    {
        AttendanceResult AttendEvent(AttendanceModel model);
        List<User> GetAttendees(int eventId);
        AttendanceResult CancelAttendance(int eventId);
        List<Attendance> GetUserAttendances(int userId);
    }
}
