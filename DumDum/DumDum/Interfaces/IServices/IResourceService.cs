using System.Collections.Generic;
using System.Threading.Tasks;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Kingdom;
using DumDum.Models.JsonEntities.Resources;

namespace DumDum.Interfaces.IServices
{
    public interface IResourceService
    {
        Task<List<ResourceList>> GetResources(int kingdomId);
        Task<Location> AddLocations(Kingdom kingdom);
        Task<(ResourceResponse, int)> ResourceLogic(int id, string authorization);
    }
}
