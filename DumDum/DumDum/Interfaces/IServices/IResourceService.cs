using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DumDum.Interfaces
{
    public interface IResourceService
    {
        Task<List<ResourceList>> GetResources(int kingdomId);
        Location AddLocations(Kingdom kingdom);
        ResourceResponse ResourceLogic(int id, string authorization, out int statusCode);
    }
}
