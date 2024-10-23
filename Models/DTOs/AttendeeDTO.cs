using System;

namespace StarterKit.Models.DTOs
{
    public class AttendeeDTO
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
    }
}