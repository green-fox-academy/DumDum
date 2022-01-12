using DumDum.Models.Entities;

namespace DumDum.Interfaces
{
    public interface IPlayerRepository : IRepository<Player>
    {
        Player GetPlayerByUsername(string username);
        bool AreCredentialsValid(string username, string password);
        Player GetPlayerById(int id);
        bool EmailNotUsed(string email);
        bool UserWithEmailExists(string username, string email);
        public Player GetPlayerWithPasswordHashed(int playerId, string hashed);
    }
}
