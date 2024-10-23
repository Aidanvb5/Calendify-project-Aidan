// Services/LoginService.cs
using StarterKit.Models;
using StarterKit.Utils;

namespace StarterKit.Services;

public enum LoginStatus { IncorrectPassword, IncorrectUsername, Success }


public class LoginService : ILoginService
{
    private readonly DatabaseContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoginService(DatabaseContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public LoginStatus CheckPassword(string username, string password)
    {
        var admin = _context.Admins.FirstOrDefault(a => a.UserName == username);
        
        if (admin == null)
            return LoginStatus.IncorrectUsername;

        string hashedPassword = EncryptionHelper.EncryptPassword(password);
        
        if (admin.Password != hashedPassword)
            return LoginStatus.IncorrectPassword;

        // Set session
        _httpContextAccessor.HttpContext?.Session.SetInt32("AdminId", admin.AdminId);
        return LoginStatus.Success;
    }

    public Admin? GetLoggedInAdmin()
    {
        var adminId = _httpContextAccessor.HttpContext?.Session.GetInt32("AdminId");
        if (!adminId.HasValue)
            return null;

        return _context.Admins.FirstOrDefault(a => a.AdminId == adminId.Value);
    }

    public bool IsAdminLoggedIn()
    {
        return _httpContextAccessor.HttpContext?.Session.GetString("adminLoggedIn") == "true";
    }

    public void Logout()
    {
        _httpContextAccessor.HttpContext?.Session.Remove("AdminId");
    }
}