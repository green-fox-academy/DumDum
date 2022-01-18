using DumDum.Database;
using DumDum.Interfaces.IRepositories;
using DumDum.Models.Entities;

namespace DumDum.Repository
{
    public class BuildingLevelRepository : Repository<BuildingLevel>, IBuildingLevelRepository
    {
        public BuildingLevelRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
