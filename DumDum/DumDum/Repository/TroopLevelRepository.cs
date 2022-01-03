using DumDum.Database;
using DumDum.Interfaces;
using DumDum.Models.Entities;
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
    }
}
