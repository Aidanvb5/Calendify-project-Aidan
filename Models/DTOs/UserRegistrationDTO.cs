using System.ComponentModel.DataAnnotations;

namespace StarterKit.Models.DTOs
{
    public class UserRegistrationDTO
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string RecuringDays { get; set; }
    }
}