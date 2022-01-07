using DumDum.Models.Entities;

namespace DumDum.Interfaces
{
    public interface IPlayerRepository : IRepository<Player>
    {
        Player GetPlayerByUsername(string username);
        bool AreCredentialsValid(string username, string password);
        Player GetPlayerById(int id);
    }
}
