// Controllers/LoginController.cs
using Microsoft.AspNetCore.Mvc;
using StarterKit.Services;

namespace StarterKit.Controllers;

[Route("api/v1/Login")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly ILoginService _loginService;

    public LoginController(ILoginService loginService)
    {
        _loginService = loginService;
    }

    [HttpPost("Login")]
    public IActionResult Login([FromBody] LoginBody loginBody)
    {
        if (string.IsNullOrEmpty(loginBody.Username) || string.IsNullOrEmpty(loginBody.Password))
            return BadRequest("Username and password are required");

        var status = _loginService.CheckPassword(loginBody.Username, loginBody.Password);
        
        switch (status)
        {
            case LoginStatus.Success:
                return Ok(new { message = "Successfully logged in" });
            case LoginStatus.IncorrectUsername:
                return Unauthorized("Username not found");
            case LoginStatus.IncorrectPassword:
                return Unauthorized("Incorrect password");
            default:
                return StatusCode(500, "An unexpected error occurred");
        }
    }

    [HttpGet("Status")]
    public IActionResult GetLoginStatus()
    {
        var admin = _loginService.GetLoggedInAdmin();
        if (admin == null)
            return Ok(new { isLoggedIn = false });

        return Ok(new { 
            isLoggedIn = true,
            username = admin.UserName
        });
    }

    [HttpGet("Logout")]
    public IActionResult Logout()
    {
        _loginService.Logout();
        return Ok(new { message = "Successfully logged out" });
    }
}

public class LoginBody
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}