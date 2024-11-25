using StarterKit.Models;
using StarterKit.Models.DTOs;
using StarterKit.Utils;

namespace StarterKit.Services;

public interface ILoginService {
    LoginStatus CheckPassword(string username, string password, string role);
    LoginStatus CheckUserPassword(string email, string password);
    Admin? GetLoggedInAdmin();
    User? GetLoggedInUser();
    bool IsAdminLoggedIn();
    bool IsUserLoggedIn();
    Task<User> RegisterUser(UserRegistrationDTO userDTO);
    void Logout();
}