using System.Threading.Tasks;
using DumDum.Models.Entities;

namespace DumDum.Interfaces
{
    public interface IPlayerRepository : IRepository<Player>
    {
        Task<Player> GetPlayerByUsername(string username);
        Task<bool> AreCredentialsValid(string username, string password);
        Task<Player> GetPlayerById(int id);
    }
}
