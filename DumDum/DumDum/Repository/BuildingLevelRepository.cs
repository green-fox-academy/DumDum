using System.Linq;
using DumDum.Database;
using DumDum.Interfaces;
using DumDum.Models.Entities;

namespace DumDum.Repository
{
    public class BuildingLevelRepository : Repository<BuildingLevel>, IBuildingLevelRepository
    {
        public BuildingLevelRepository(ApplicationDbContext context) : base(context)
        {
        }

        public int GetProductionByBuildingTypeAndLevel(int buildingTypeId, int buildingLevel)
        {
            var result = DbContext.BuildingLevels
                .FirstOrDefault(b => b.BuildingLevelId == buildingTypeId && b.LevelNumber == buildingLevel);
            if (result is not null)
            {
                return result.Production;
            }

            return 0;
        }

        public int GetConsumptionByBuildingTypeAndLevel(int buildingTypeId, int buildingLevel)
        {
            var result = DbContext.BuildingLevels
                .FirstOrDefault(b => b.BuildingLevelId == buildingTypeId && b.LevelNumber == buildingLevel);
            if (result is not null)
            {
                return result.Consumption;
            }

            return 0;
        }
    }
}