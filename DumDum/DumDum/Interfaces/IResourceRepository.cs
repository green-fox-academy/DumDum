using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using System.Collections.Generic;

namespace DumDum.Interfaces
{
    public interface IResourceRepository : IRepository<Resource>
    {
        Resource GetGoldAmountOfKingdom(int kingdomId);
        void UpdateGoldAmountOfKingdom(Resource resource);
        List<ResourceList> GetResources(int id);
    }
}
