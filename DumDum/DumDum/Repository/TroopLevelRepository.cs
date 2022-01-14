using DumDum.Database;
using DumDum.Interfaces;
using DumDum.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DumDum.Repository
{
    public class TroopLevelRepository : Repository<TroopLevel>, ITroopLevelRepository
    {
        public TroopLevelRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<int> MaximumLevelPossible()
        {
            var number = DbContext.TroopLevel.Select(t => t.Level).Max();
            return number;
        }

        public async Task<TroopLevel> TroopCreationHigherLevel(string troopType, int troopCreationLevel)
        {
            var level = DbContext.TroopLevel
                .Include(t => t.TroopType)
                .FirstOrDefault(t => t.TroopType.TroopType == troopType.ToLower() && t.Level == troopCreationLevel);
            return level;
        }
        
        public int GetConsumptionByTroopTypeAndLevel(int troopTypeId, int troopLevel)
        {
            var result = DbContext.TroopLevel
                .FirstOrDefault(t => t.TroopTypeId == troopTypeId && t.Level == troopLevel);
            if (result is not null)
            {
                return (int)result.Consumption;
            }

            return 0;
        }
    }
}
