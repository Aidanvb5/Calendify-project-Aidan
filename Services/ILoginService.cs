using StarterKit.Models;

namespace StarterKit.Services;
public interface ILoginService
{
    LoginResult Login(string username, string password);
    LoginStatus GetLoginStatus();
    RegistrationResult Register(RegisterModel model);
    LoginResult AdminLogin(string username, string password); // This method is missing in the implementation
}