using DumDum.Database;
using DumDum.Interfaces;
using DumDum.Models.Entities;

namespace DumDum.Repository
{
    public class ResourceRepository : Repository<Resource>, IResourceRepository
    {
        public ResourceRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
