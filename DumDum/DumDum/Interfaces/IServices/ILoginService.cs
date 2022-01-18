using System.Threading.Tasks;
using DumDum.Models.JsonEntities.Login;

namespace DumDum.Interfaces.IServices
{
    public interface ILoginService
    {
        Task<(string, int)> Login(LoginRequest player);
        Task<bool> LoginCheck(string username);
        Task<bool> PasswordCheck(string password);
        Task<bool> LoginPasswordCheck(string username, string password);
        Task<string> Authenticate(string username, string password);
    }
}
