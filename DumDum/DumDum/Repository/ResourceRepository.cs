using DumDum.Database;
using DumDum.Interfaces;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Resources;
using System.Collections.Generic;
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
        
        public Resource GetFoodAmountOfKingdom(int kingdomId)
        {
            return DbContext.Resources.FirstOrDefault(r => r.KingdomId == kingdomId && r.ResourceType == "Food");
        }

        public void UpdateGoldAmountOfKingdom(Resource gold)
        {
            DbContext.Resources.Update(gold);
        }
        public List<ResourceList> GetResources(int id)
        {
            return DbContext.Resources.Where(r => r.KingdomId == id).Select(r => new ResourceList()
            {
                ResourceType = r.ResourceType,
                Amount = r.Amount,
                Generation = r.Generation,
                UpdatedAt = r.UpdatedAt
            }).ToList();
        }
    }
}
