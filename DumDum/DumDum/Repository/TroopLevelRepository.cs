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
            return await Task.FromResult(number);
        }

        public async Task<TroopLevel> TroopCreationHigherLevel(string troopType, int troopCreationLevel)
        {
            var level = DbContext.TroopLevel
                .Include(t => t.TroopType)
                .FirstOrDefault(t => t.TroopType.TroopType == troopType.ToLower() && t.Level == troopCreationLevel);
            return await Task.FromResult(level);
        }
    }
}
