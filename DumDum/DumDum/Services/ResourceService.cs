using DumDum.Database;

namespace DumDum.Services
{
    public class ResourceService
    {
        private ApplicationDbContext DbContext { get; set; }

        public ResourceService(ApplicationDbContext dbContex)
        {
            DbContext = dbContex;
        }
    }
}