using System.Collections.Generic;
using System.Threading.Tasks;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Troops;

namespace DumDum.Interfaces.IRepositories
{
    public interface ITroopRepository : IRepository<Troop>
    {
        Task<List<TroopsResponse>> GetTroops(int kingdomId);
        void UpgradeTroops(int troopTypeIdToBeUpgraded, int kingdomId, int timeRequiredToUpgradeTroop);
        Task<int> FinishedAtTimeTroop(string troopType, int kingdomId);
        Task<List<Troop>> GetAllTroopsOfKingdom(int kingdomId);
    }
}
