using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using System.Collections.Generic;

namespace DumDum.Interfaces
{
    public interface ITroopRepository : IRepository<Troop>
    {
        List<TroopsResponse> GetTroops(int kingdomId);
        void UpgradeTroops(int troopTypeIdToBeUpgraded, int kingdomId, int timeRequiredToUpgradeTroop);
        int FinishedAtTimeTroop(string troopType, int kingdomId);
    }
}
