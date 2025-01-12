namespace StarterKit.Models
{
    public class Review
    {
        public int Id { get; set; }           // Identifier for Review
        public int EventId { get; set; }      // Foreign key reference to Event
        public int UserId { get; set; }    // User identifier
        public int Rating { get; set; }        // Rating value (1-5)
        public string Comment { get; set; }    // User's comment
        public DateTime CreatedDate { get; set; } // Date when review was created

        // Navigation properties
        public required User User { get; set; }
        public required Event Event { get; set; }

        // Validation
        public bool IsValid => Rating >= 1 && Rating <= 5;
    }
}
