using StarterKit.Models;
using StarterKit.Models.DTOs;
using StarterKit.Utils;

namespace StarterKit.Services;

public interface ILoginService {
    LoginStatus CheckPassword(string username, string password, string role);
    Admin? GetLoggedInAdmin();
    User? GetLoggedInUser();
    bool IsAdminLoggedIn();
    bool IsUserLoggedIn();
    Task<User> RegisterUser(UserLoginDTO userDTO);
    void Logout();
}