using System;

namespace StarterKit.Models.DTOs
{
    public class ReviewDTO
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedDate { get; set; } // Date when review was created
        public int UserId { get; set; } // User who created the review
        public int EventId { get; set; } // Event associated with the review
    }
}
