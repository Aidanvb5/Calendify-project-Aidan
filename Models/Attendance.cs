namespace StarterKit.Models
{
    public class Attendance
    {
        public int AttendanceId { get; set; }

        public DateTime AttendanceDate { get; set; }

        public required User User { get; set; }

        public string Status { get; set; } = "Tentative"; // Default status: Tentative, Present, Absent

        public string OfficeLocation { get; set; } = "Main Office"; // Default location

        public bool IsValid => AttendanceDate.Date >= DateTime.Today.Date; // Check if attendance date is in the future
    }
}
