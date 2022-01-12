using System.Threading.Tasks;
using DumDum.Models.JsonEntities.Login;

namespace DumDum.Interfaces
{
    public interface ILoginService
    {
        string Login(LoginRequest player, out int statusCode);
        Task<bool> LoginCheck(string username);
        Task<bool> PasswordCheck(string password);
        Task<bool> LoginPasswordCheck(string username, string password);
        Task<string> Authenticate(string username, string password);
    }
}
