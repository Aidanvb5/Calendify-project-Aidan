namespace StarterKit.Models.DTOs
{
    public class AttendeeDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Status { get; set; } = "Tentative"; // Default status
    }
}
