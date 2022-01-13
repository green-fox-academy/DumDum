using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Buildings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DumDum.Interfaces
{
    public interface IBuildingTypeRepository : IRepository<BuildingType>
    {
        Task<BuildingType> FindLevelingByBuildingType(string buildingType);
        Task<List<string>> ExistingTypeOfBuildings();
    }
}
