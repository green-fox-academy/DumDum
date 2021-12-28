using DumDum.Database;
using DumDum.Interfaces;
using DumDum.Models.Entities;

namespace DumDum.Repository
{
    public class BuildingRepository : Repository<Building>, IBuildingRepository
    {
        public BuildingRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
