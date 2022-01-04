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

        public List<TroopsResponse> GetTroops(int kingdomId)
        {
            List<TroopsResponse> troops = DbContext.Troops.Where(t => t.KingdomId == kingdomId).Include(t => t.TroopType.TroopLevel).ToList().
                Select(t => new TroopsResponse()
                {
                    TroopId = t.TroopId,
                    TroopType = t.TroopType.TroopType,
                    Level = t.Level,
                    HP = t.TroopType.TroopLevel.Level,
                    Attack = t.TroopType.TroopLevel.Attack,
                    Defence = t.TroopType.TroopLevel.Defence,
                    StartedAt = t.StartedAt,
                    FinishedAt = t.FinishedAt
                }).ToList();
            if (troops != null)
            {
                return troops;
            }
            return new List<TroopsResponse>();
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

        public int FinishedAtTimeTroop(string troopType, int kingdomId)
        {
            return DbContext.Troops.Include(t => t.TroopType)
                .Where(t => t.TroopType.TroopType.ToLower() == troopType.ToLower() && t.KingdomId == kingdomId)
                .Select(t => t.FinishedAt).FirstOrDefault();
        }
    }
}
