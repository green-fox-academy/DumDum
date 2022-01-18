using System.Threading.Tasks;
using DumDum.Models.Entities;

namespace DumDum.Interfaces.IRepositories
{
    public interface IPlayerRepository : IRepository<Player>
    {
        Task<Player> GetPlayerByUsername(string username);
        Task<bool> AreCredentialsValid(string username, string password);
        Task<Player> GetPlayerById(int id);
        bool EmailNotUsed(string email);
        bool UserWithEmailExists(string username, string email);
        public Player GetPlayerWithPasswordHashed(int playerId, string hashed);
    }
}
