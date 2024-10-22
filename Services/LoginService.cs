using StarterKit.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;

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
        if (user == null)
        {
            return new LoginResult { Success = false, Message = "Invalid username or password" };
        }

        var hashedPassword = EncryptionHelper.HashPassword(password);
        if (user.Password != hashedPassword)
        {
            return new LoginResult { Success = false, Message = "Invalid username or password" };
        }

        // Set session
        _httpContextAccessor.HttpContext?.Session.SetString("UserId", user.UserId.ToString());
        _httpContextAccessor.HttpContext?.Session.SetString("UserRole", user.Role);
        _httpContextAccessor.HttpContext?.Session.SetString("Username", user.Username);

        return new LoginResult 
        { 
            Success = true, 
            User = user,
            Message = "Login successful"
        };
    }

    public LoginStatus GetLoginStatus()
    {
        var userId = _httpContextAccessor.HttpContext?.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userId))
        {
            return new LoginStatus { IsLoggedIn = false };
        }

        var user = _context.User.Find(int.Parse(userId));
        if (user == null)
        {
            return new LoginStatus { IsLoggedIn = false };
        }

        return new LoginStatus 
        { 
            IsLoggedIn = true, 
            Username = user.Username 
        };
    }

    public RegistrationResult Register(RegisterModel model)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(model.Username) || 
            string.IsNullOrWhiteSpace(model.Password) || 
            string.IsNullOrWhiteSpace(model.Email))
        {
            return new RegistrationResult 
            { 
                Success = false, 
                Message = "All fields are required" 
            };
        }

        // Check if username already exists
        if (_context.User.Any(u => u.Username == model.Username))
        {
            return new RegistrationResult 
            { 
                Success = false, 
                Message = "Username already exists" 
            };
        }

        // Check if email already exists
        if (_context.User.Any(u => u.Email == model.Email))
        {
            return new RegistrationResult 
            { 
                Success = false, 
                Message = "Email already exists" 
            };
        }

        var user = new User
        {
            Username = model.Username,
            Password = EncryptionHelper.HashPassword(model.Password),
            Email = model.Email,
            Role = "User", // Default role
            FirstName = model.FirstName ?? "", // Add these properties to RegisterModel
            LastName = model.LastName ?? "",
            RecuringDays = "" // Empty by default
        };

        try
        {
            _context.User.Add(user);
            _context.SaveChanges();

            return new RegistrationResult 
            { 
                Success = true,
                Message = "Registration successful" 
            };
        }
        catch (Exception ex)
        {
            return new RegistrationResult 
            { 
                Success = false,
                Message = "An error occurred during registration" 
            };
        }
    }

    private static bool VerifyPassword(string inputPassword, string storedPassword)
    {
        var hashedInput = EncryptionHelper.HashPassword(inputPassword);
        return hashedInput == storedPassword;
    }
}


