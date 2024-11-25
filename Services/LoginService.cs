using StarterKit.Models;
using StarterKit.Models.DTOs;
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

    public LoginStatus CheckPassword(string username, string password, string role)
    {
        if (role.ToLower() == "admin")
        {
            var admin = _context.Admins.FirstOrDefault(a => a.UserName == username);
            if (admin == null)
                return LoginStatus.IncorrectUsername;

            string hashedPassword = EncryptionHelper.EncryptPassword(password);
            if (admin.Password != hashedPassword)
                return LoginStatus.IncorrectPassword;

            // Set admin session
            _httpContextAccessor.HttpContext?.Session.SetInt32("AdminId", admin.AdminId);
            _httpContextAccessor.HttpContext?.Session.SetString("Role", "Admin");
            return LoginStatus.Success;
        }
        else
        {
            return CheckUserPassword(username, password);
        }
    }

    public LoginStatus CheckUserPassword(string email, string password)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        if (user == null)
            return LoginStatus.IncorrectUsername;

        string hashedPassword = EncryptionHelper.EncryptPassword(password);
        if (user.Password != hashedPassword)
            return LoginStatus.IncorrectPassword;

        // Set user session
        _httpContextAccessor.HttpContext?.Session.SetInt32("UserId", user.UserId);
        _httpContextAccessor.HttpContext?.Session.SetString("Role", "User");
        return LoginStatus.Success;
    }

    public async Task<User> RegisterUser(UserRegistrationDTO userDTO)
    {
        // Check if user already exists
        if (_context.Users.Any(u => u.Email == userDTO.Email))
        {
            throw new Exception("User with this email already exists");
        }

        var user = new User
        {
            Email = userDTO.Email,
            Password = EncryptionHelper.EncryptPassword(userDTO.Password),
            FirstName = userDTO.FirstName,
            LastName = userDTO.LastName,
            RecuringDays = "", // Set default value
            Attendances = new List<Attendance>(),
            Event_Attendances = new List<Event_Attendance>()
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public Admin? GetLoggedInAdmin()
    {
        var adminId = _httpContextAccessor.HttpContext?.Session.GetInt32("AdminId");
        if (!adminId.HasValue)
            return null;

        return _context.Admins.FirstOrDefault(a => a.AdminId == adminId.Value);
    }

    public User? GetLoggedInUser()
    {
        var userId = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return null;

        return _context.Users.FirstOrDefault(u => u.UserId == userId.Value);
    }

    public bool IsAdminLoggedIn()
    {
        return _httpContextAccessor.HttpContext?.Session.GetString("Role") == "Admin";
    }

    public bool IsUserLoggedIn()
    {
        return _httpContextAccessor.HttpContext?.Session.GetString("Role") == "User";
    }

    public void Logout()
    {
        _httpContextAccessor.HttpContext?.Session.Clear();
    }
}