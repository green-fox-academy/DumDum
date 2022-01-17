using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DumDum.Interfaces
{
    public interface ITroopRepository : IRepository<Troop>
    {
        Task<List<TroopsResponse>> GetTroops(int kingdomId);
        void UpgradeTroops(int troopTypeIdToBeUpgraded, int kingdomId, int timeRequiredToUpgradeTroop);
        Task<int> FinishedAtTimeTroop(string troopType, int kingdomId);
        Task<List<Troop>> GetAllTroopsOfKingdom(int kingdomId);
    }
}
