using System.Collections.Generic;
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

        public Dictionary<string, int> AddToDictionary(int coordinateX, int coordinateY)
        {
            Dictionary<string, int> dictionaryToReturn = new Dictionary<string, int>();
            dictionaryToReturn.Add("CoordinateX", coordinateX);
            dictionaryToReturn.Add("CoordinateY", coordinateY);
            return dictionaryToReturn;
        }
    }
}