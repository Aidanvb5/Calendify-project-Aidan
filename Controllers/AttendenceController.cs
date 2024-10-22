using Microsoft.AspNetCore.Mvc;
using StarterKit.Models;
using StarterKit.Services;

namespace StarterKit.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;

    public AttendanceController(IAttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    [HttpPost]
    public IActionResult AttendEvent([FromBody] AttendanceModel model)
    {
        var userId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Must be logged in to attend events" });
        }

        // Ensure user can only create attendance for themselves
        model.UserId = int.Parse(userId);

        var result = _attendanceService.AttendEvent(model);
        if (result.Success)
        {
            return Ok(new { message = "Attendance registered successfully" });
        }
        return BadRequest(new { message = result.Message });
    }

    [HttpGet]
    public IActionResult GetAttendees(int eventId)
    {
        var userId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Must be logged in to view attendees" });
        }

        var attendees = _attendanceService.GetAttendees(eventId);
        return Ok(attendees);
    }

    [HttpDelete("{eventId}")]
    public IActionResult CancelAttendance(int eventId)
    {
        var userId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Must be logged in to cancel attendance" });
        }

        var result = _attendanceService.CancelAttendance(eventId);
        if (result.Success)
        {
            return Ok(new { message = "Attendance cancelled successfully" });
        }
        return BadRequest(new { message = result.Message });
    }

    [HttpGet("my-attendances")]
    public IActionResult GetMyAttendances()
    {
        var userId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Must be logged in to view your attendances" });
        }

        var attendances = _attendanceService.GetUserAttendances(int.Parse(userId));
        return Ok(attendances);
    }
}