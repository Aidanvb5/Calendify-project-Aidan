namespace StarterKit.Models
{
    public class User
    {
        public int UserId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Username { get; set; } // Add this property
        public required string Role { get; set; } // Add this property
        public required string RecuringDays { get; set; }
        public List<Attendance> Attendances { get; set; } = new();
        public List<Event_Attendance> Event_Attendances { get; set; } = new();

        // Remove this implicit operator as it's causing recursion
        /*public static implicit operator User(User v)
        {
            throw new NotImplementedException();
        }*/
    }
    }

    public class Attendance
    {
        public int AttendanceId { get; set; }

        public DateTime AttendanceDate { get; set; }

        public required User User { get; set; }
        public int EventId { get; internal set; }

        public static implicit operator Attendance(Attendance v)
        {
            throw new NotImplementedException();
        }
    }

    public class Event_Attendance
    {
        public int Event_AttendanceId { get; set; }
        public int Rating { get; set; }
        public required string Feedback { get; set; }
        public required User User { get; set; }
        public required Event Event { get; set; }
    }

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
        public object Reviews { get; internal set; }
        public object Attendees { get; internal set; }
    }
}