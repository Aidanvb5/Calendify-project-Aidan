using StarterKit.Models;
using StarterKit.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarterKit.Services
{
    public interface IEventService
    {
        Task<IEnumerable<EventDTO>> GetEventsAsync();
        Task<EventDTO> CreateEventAsync(EventCreateDTO eventCreateDTO);
        Task<EventDTO> UpdateEventAsync(int id, EventUpdateDTO eventUpdateDTO);
        Task DeleteEventAsync(int id);
        Task<ReviewDTO> CreateReviewAsync(int eventId, ReviewCreateDTO reviewCreateDTO);
    }
}