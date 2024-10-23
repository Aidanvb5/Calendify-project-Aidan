namespace StarterKit.Models.DTOs
{
    public class EventCreateDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateOnly EventDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Location { get; set; }
    }
}