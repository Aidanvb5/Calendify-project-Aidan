namespace StarterKit.Models
{
    public class Event
    {
        public int EventId { get; set; }

        public required string Title { get; set; }

        public required string Description { get; set; }

        public DateTime EventDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public required string Location { get; set; }

        public bool AdminApproval { get; set; }

        public required List<Event_Attendance> Event_Attendances { get; set; }

        // Additional properties for validation
        public int Capacity { get; set; } // Maximum number of attendees
        public bool IsAvailable => EventDate.Date >= DateTime.Today.Date; // Check if the event is in the future
    }
}
