using StarterKit.Models;

namespace StarterKit.Services
{
	public interface ILoginService
	{
		LoginResult Login(string username, string password);
		LoginStatus GetLoginStatus();
		RegistrationResult Register(RegisterModel model);
		// Add new method for admin login
		LoginResult AdminLogin(string username, string password);
	}
}