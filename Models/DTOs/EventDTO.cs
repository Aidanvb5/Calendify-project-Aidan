
using System;
using System.Collections.Generic;

namespace StarterKit.Models.DTOs
{
    public class EventDTO
    {
        public int Id { get; set; }
        public string Title { get; set ; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Location { get; set; }
        public IEnumerable<ReviewDTO> Reviews { get; set; }
        public IEnumerable<AttendeeDTO> Attendees { get; set; }
    }
}