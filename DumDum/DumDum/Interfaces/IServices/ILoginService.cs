using DumDum.Models.JsonEntities.Login;

namespace DumDum.Interfaces
{
    public interface ILoginService
    {
        string Login(LoginRequest player, out int statusCode);
        bool LoginCheck(string username);
        bool PasswordCheck(string password);
        bool LoginPasswordCheck(string username, string password);
        string Authenticate(string username, string password);
    }
}
