using Microsoft.AspNetCore.Mvc;
using StarterKit.Models.DTOs;
using StarterKit.Services;
using StarterKit.Utils;

namespace StarterKit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost("admin")]
        public IActionResult AdminLogin([FromBody] LoginDTO loginDto)
        {
            var status = _loginService.CheckPassword(loginDto.Username, loginDto.Password, "admin");
            
            return status switch
            {
                LoginStatus.Success => Ok(new { message = "Admin login successful" }),
                LoginStatus.IncorrectUsername => BadRequest(new { message = "Admin username not found" }),
                LoginStatus.IncorrectPassword => BadRequest(new { message = "Incorrect admin password" }),
                _ => BadRequest(new { message = "Admin login failed" })
            };
        }

        [HttpPost("user")]
        public IActionResult UserLogin([FromBody] LoginDTO loginDto)
        {
            var status = _loginService.CheckPassword(loginDto.Username, loginDto.Password, "user");
            
            return status switch
            {
                LoginStatus.Success => Ok(new { message = "User login successful" }),
                LoginStatus.IncorrectUsername => BadRequest(new { message = "User email not found" }),
                LoginStatus.IncorrectPassword => BadRequest(new { message = "Incorrect user password" }),
                _ => BadRequest(new { message = "User login failed" })
            };
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationDTO registrationDto)
        {
            try
            {
                var user = await _loginService.RegisterUser(registrationDto);
                return CreatedAtAction(nameof(RegisterUser), new { id = user.UserId }, 
                    new { message = "User registered successfully", userId = user.UserId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("check-session")]
        public IActionResult CheckSession()
        {
            if (_loginService.IsAdminLoggedIn())
            {
                var admin = _loginService.GetLoggedInAdmin();
                return Ok(new 
                { 
                    isLoggedIn = true, 
                    role = "Admin", 
                    name = admin?.UserName 
                });
            }
            else if (_loginService.IsUserLoggedIn())
            {
                var user = _loginService.GetLoggedInUser();
                return Ok(new 
                { 
                    isLoggedIn = true, 
                    role = "User", 
                    name = $"{user?.FirstName} {user?.LastName}" 
                });
            }

            return Ok(new 
            { 
                isLoggedIn = false, 
                role = (string)null, 
                name = (string)null 
            }
            );
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            _loginService.Logout();
            return Ok(new { message = "Logged out successfully" });
        }
    }
}