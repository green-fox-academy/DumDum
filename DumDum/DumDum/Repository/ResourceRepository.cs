using DumDum.Database;
using DumDum.Interfaces;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Resources;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DumDum.Repository
{
    public class ResourceRepository : Repository<Resource>, IResourceRepository
    {
        public ResourceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Resource> GetGoldAmountOfKingdom(int kingdomId)
        {
           var resource = DbContext.Resources.FirstOrDefault(r => r.KingdomId == kingdomId && r.ResourceType == "Gold");
           return resource;
        }
        
        public async Task<Resource> GetFoodAmountOfKingdom(int kingdomId)
        {
            return DbContext.Resources.FirstOrDefault(r => r.KingdomId == kingdomId && r.ResourceType == "Food");
        }

        public void UpdateGoldAmountOfKingdom(Resource gold)
        {
            DbContext.Resources.Update(gold);
        }

        public async Task<List<ResourceList>> GetResources(int id)
        {
            var resList =  DbContext.Resources.Where(r => r.KingdomId == id).Select(r => new ResourceList(r)).ToList();
            return resList;
        }
    }
}
