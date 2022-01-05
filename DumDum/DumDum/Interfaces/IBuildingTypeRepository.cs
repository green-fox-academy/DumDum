using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Buildings;
using System.Collections.Generic;

namespace DumDum.Interfaces
{
    public interface IBuildingTypeRepository : IRepository<BuildingType>
    {
        BuildingType FindLevelingByBuildingType(string buildingType);
        List<string> ExistingTypeOfBuildings();
    }
}
