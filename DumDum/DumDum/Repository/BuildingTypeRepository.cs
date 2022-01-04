using DumDum.Database;
using DumDum.Interfaces;
using DumDum.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DumDum.Repository
{
    public class BuildingTypeRepository : Repository<BuildingType>, IBuildingTypeRepository
    {
        public BuildingTypeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public BuildingType FindLevelingByBuildingType(string buildingType)
        {
            return DbContext.BuildingTypes.Include(b => b.BuildingLevels)
                .FirstOrDefault(p => p.BuildingTypeName == buildingType);
        }

        public List<string> ExistingTypeOfBuildings()
        {
            return DbContext.BuildingTypes.Select(b => b.BuildingTypeName.ToLower()).ToList();
        }
    }
}
