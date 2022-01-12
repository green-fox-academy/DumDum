using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Buildings;
using System.Collections.Generic;

namespace DumDum.Interfaces
{
    public interface IBuildingLevelRepository : IRepository<BuildingLevel>
    {
        int GetProductionByBuildingTypeAndLevel(int buildingTypeId, int buildingLevel);
        int GetConsumptionByBuildingTypeAndLevel(int buildingTypeId, int buildingLevel);
    }
}