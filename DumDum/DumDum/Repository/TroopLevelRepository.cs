using DumDum.Database;
using DumDum.Interfaces;
using DumDum.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DumDum.Repository
{
    public class TroopLevelRepository : Repository<TroopLevel>, ITroopLevelRepository
    {
        public TroopLevelRepository(ApplicationDbContext context) : base(context)
        {
        }

        public int MaximumLevelPossible()
        {
            return DbContext.TroopLevel.Select(t => t.Level).Max();
        }

        public TroopLevel TroopCreationHigherLevel(string troopType, int troopCreationLevel)
        {
            return DbContext.TroopLevel.Include(t => t.TroopType)
                    .Where(t => t.TroopType.TroopType == troopType.ToLower() && t.Level == troopCreationLevel)
                    .FirstOrDefault();
        }
    }
}
