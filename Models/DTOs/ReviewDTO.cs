using System;

namespace StarterKit.Models.DTOs
{
    public class ReviewDTO
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}