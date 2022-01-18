using DumDum.Models.Entities;

namespace DumDum.Interfaces
{
    public interface IBuildingLevelRepository : IRepository<BuildingLevel>
    {
        int GetProductionByBuildingTypeAndLevel(int buildingTypeId, int buildingLevel);
        int GetConsumptionByBuildingTypeAndLevel(int buildingTypeId, int buildingLevel);
    }
}