using DumDum.Database;
using DumDum.Interfaces;
using DumDum.Models.Entities;
using System.Linq;

namespace DumDum.Repository
{
    public class ResourceRepository : Repository<Resource>, IResourceRepository
    {
        public ResourceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Resource GetGoldAmountOfKingdom(int kingdomId)
        {
           return DbContext.Resources.FirstOrDefault(r => r.KingdomId == kingdomId && r.ResourceType == "Gold");
        }

        public void UpdateGoldAmountOfKingdom(Resource gold)
        {
            DbContext.Resources.Update(gold);
        }
    }
}
