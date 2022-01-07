using DumDum.Database;
using DumDum.Interfaces;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DumDum.Repository
{
    public class TroopRepository : Repository<Troop>, ITroopRepository
    {
        public TroopRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<TroopsResponse>> GetTroops(int kingdomId)
        {
            var troops =  DbContext.Troops.Where(t => t.KingdomId == kingdomId).Include(t => t.TroopType.TroopLevel).Select(t => new TroopsResponse(t)).ToList();
            if (troops != null)
            {
                return troops;
            }
            return await Task.FromResult(troops);
        }

        public void UpgradeTroops(int troopTypeIdToBeUpgraded, int kingdomId, int timeRequiredToUpgradeTroop)
        {
            DbContext.Troops.Where(t => t.TroopTypeId == troopTypeIdToBeUpgraded && t.KingdomId == kingdomId).ToList()
                     .ForEach(t =>
                     {
                         t.Level++;
                         t.StartedAt = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
                         t.FinishedAt = (int)DateTimeOffset.Now.ToUnixTimeSeconds() + timeRequiredToUpgradeTroop;
                     });
        }

        public async Task<int> FinishedAtTimeTroop(string troopType, int kingdomId)
        {
            var number =  DbContext.Troops.Include(t => t.TroopType)
                .Where(t => t.TroopType.TroopType.ToLower() == troopType.ToLower() && t.KingdomId == kingdomId)
                .Select(t => t.FinishedAt).FirstOrDefault();
            return await Task.FromResult(number);
        }
    }
}
