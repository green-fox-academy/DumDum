using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DumDum.Interfaces
{
    public interface IResourceRepository : IRepository<Resource>
    {
        Task<Resource> GetGoldAmountOfKingdom(int kingdomId);
        Task<Resource> GetFoodAmountOfKingdom(int kingdomId);
        void UpdateGoldAmountOfKingdom(Resource resource);
        void UpdateFoodAmountOfKingdom(Resource resource);
        Task<List<ResourceList>> GetResources(int id);
    }
}
