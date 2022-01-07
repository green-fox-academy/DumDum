using DumDum.Database;
using DumDum.Interfaces;
using DumDum.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DumDum.Repository
{
    public class BuildingTypeRepository : Repository<BuildingType>, IBuildingTypeRepository
    {
        public BuildingTypeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<BuildingType> FindLevelingByBuildingType(string buildingType)
        {
            var type =  DbContext.BuildingTypes.Include(b => b.BuildingLevels)
                .FirstOrDefault(p => p.BuildingTypeName == buildingType);
            return await Task.FromResult(type);
        }

        public async Task<List<string>> ExistingTypeOfBuildings()
        {
            var list =  DbContext.BuildingTypes.Select(b => b.BuildingTypeName.ToLower()).ToList();
            return await Task.FromResult(list);
        }
    }
}
