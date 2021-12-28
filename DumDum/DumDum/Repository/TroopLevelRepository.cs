using DumDum.Database;
using DumDum.Interfaces;
using DumDum.Models.Entities;

namespace DumDum.Repository
{
    public class TroopLevelRepository : Repository<TroopLevel>, ITroopLevelRepository
    {
        public TroopLevelRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
