using StarterKit.Models;
using StarterKit.Utils;

namespace StarterKit.Services;

public interface ILoginService {
    LoginStatus CheckPassword(string username, string password);
    Admin? GetLoggedInAdmin();
    public bool IsAdminLoggedIn();
    void Logout();
}