using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Resources;
using System.Collections.Generic;

namespace DumDum.Interfaces
{
    public interface IResourceRepository : IRepository<Resource>
    {
        Resource GetGoldAmountOfKingdom(int kingdomId);
        Resource GetFoodAmountOfKingdom(int kingdomId);
        void UpdateGoldAmountOfKingdom(Resource resource);
        List<ResourceList> GetResources(int id);
    }
}
