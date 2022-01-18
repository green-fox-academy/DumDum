using System.Collections.Generic;
using System.Threading.Tasks;
using DumDum.Models.Entities;

namespace DumDum.Interfaces.IRepositories
{
    public interface IBuildingTypeRepository : IRepository<BuildingType>
    {
        Task<BuildingType> FindLevelingByBuildingType(string buildingType);
        Task<List<string>> ExistingTypeOfBuildings();
    }
}
