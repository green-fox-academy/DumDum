using DumDum.Database;
using DumDum.Interfaces;
using DumDum.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DumDum.Repository
{
    public class KingdomRepository : Repository<Kingdom>, IKingdomRepository
    {
        public KingdomRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Kingdom GetKingdomByName(string kingdomName)
        {
            return DbContext.Kingdoms.Include(k => k.Player).FirstOrDefault(x => x.KingdomName == kingdomName);
        }
    }
}

