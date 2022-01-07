using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Buildings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DumDum.Interfaces
{
    public interface IKingdomRepository : IRepository<Kingdom>
    {
        Task<Kingdom> GetKingdomByName(string kingdomName);
        Task<Kingdom> GetKingdomById(int kingdomId);
        Task<KingdomsListResponse> GetAllKingdoms();
        Task<Kingdom> FindPlayerByKingdomId(int kingdomId);
        Task<List<Kingdom>> GetAllKingdomsIncludePlayer();
        Task<List<BuildingPoints>> GetListBuildingPoints();

    }
}
