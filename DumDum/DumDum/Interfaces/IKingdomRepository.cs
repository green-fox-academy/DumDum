using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;

namespace DumDum.Interfaces
{
    public interface IKingdomRepository : IRepository<Kingdom>
    {
        Kingdom GetKingdomByName(string kingdomName);
        Kingdom GetKingdomById(int kingdomId);
        KingdomsListResponse GetAllKingdoms();

    }
}
