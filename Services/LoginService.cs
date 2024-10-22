using StarterKit.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;
using StarterKit.Utils;

namespace StarterKit.Services;


public class LoginService : ILoginService
{
    private readonly DatabaseContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoginService(DatabaseContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public LoginResult Login(string username, string password)
    {
        var user = _context.User.FirstOrDefault(u => u.Username == username);
        if (user == null || !VerifyPassword(password, user.Password))
        {
            return new LoginResult { Success = false, Message = "Invalid username or password" };
        }

        _httpContextAccessor.HttpContext.Session.SetString("UserId", user.UserId.ToString());
        _httpContextAccessor.HttpContext.Session.SetString("UserRole", user.Role);

        return new LoginResult { Success = true, User = user };
    }

    // Add the missing AdminLogin method
    public LoginResult AdminLogin(string username, string password)
    {
        var admin = _context.Admin.FirstOrDefault(a => a.UserName == username);
        if (admin == null || !VerifyPassword(password, admin.Password))
        {
            return new LoginResult { Success = false, Message = "Invalid admin credentials" };
        }

        _httpContextAccessor.HttpContext.Session.SetString("AdminId", admin.AdminId.ToString());
        _httpContextAccessor.HttpContext.Session.SetString("UserRole", "Admin");

        return new LoginResult { Success = true, Message = "Admin login successful" };
    }

    public LoginStatus GetLoginStatus()
    {
        var userId = _httpContextAccessor.HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userId))
        {
            return new LoginStatus { IsLoggedIn = false };
        }

        var user = _context.User.Find(int.Parse(userId));
        return new LoginStatus { IsLoggedIn = true, Username = user.Username };
    }

    public RegistrationResult Register(RegisterModel model)
    {
        if (_context.User.Any(u => u.Username == model.Username))
        {
            return new RegistrationResult { Success = false, Message = "Username already exists" };
        }

        var user = new User
        {
            Username = model.Username,
            Password = StorePassword(model.Password),
            Email = model.Email,
            Role = "User",
            FirstName = "", // Add required properties
            LastName = "",
            RecuringDays = "",
            Attendances = new List<Attendance>(),
            Event_Attendances = new List<Event_Attendance>()
        };

        _context.User.Add(user);
        _context.SaveChanges();

        return new RegistrationResult { Success = true };
    }

    private static bool VerifyPassword(string inputPassword, string storedPassword)
    {
        var hashedInput = EncryptionHelper.HashPassword(inputPassword);
        return hashedInput == storedPassword;
    }

	private string StorePassword(string password)
	{
		return Convert.ToBase64String(HashPassword(password));
	}
}


