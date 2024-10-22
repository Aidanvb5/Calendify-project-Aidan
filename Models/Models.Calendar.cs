using System;
using System.Collections.Generic;

namespace StarterKit.Models
{
    public class User
    {
        public int UserId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string RecuringDays { get; set; }
        public required List<Attendance> Attendances { get; set; } = new List<Attendance>(); // Initialize list
        public required List<Event_Attendance> EventAttendances { get; set; } = new List<Event_Attendance>(); // Initialize list
        public string Username { get; set; } // Changed from internal
        public string Role { get; set; } // Changed from internal
    }

    public class Attendance
    {
        public int AttendanceId { get; set; }
        public int UserId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public required User User { get; set; } // Required property
        public int EventId { get; set; }
        public required Event Event { get; set; } // Required property
        public int Rating { get; set; } // Assuming you have a rating property
        public string Feedback { get; set; } // Assuming you have a feedback property
    }

    public class Event_Attendance
    {
        public int Event_AttendanceId { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
        public int Rating { get; set; }
        public required string Feedback { get; set; }
        public required User User { get; set; } // Required property
        public required Event Event { get; set; } // Required property
    }

    public class Event
    {
        public int EventId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public DateOnly EventDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public required string Location { get; set; }
        public bool AdminApproval { get; set; }
        public required List<Event_Attendance> EventAttendances { get; set; } = new List<Event_Attendance>(); // Initialize list
        public List<User> Attendees { get; set; } = new List<User>(); // Initialize list
    }
}