using DumDum.Database;
using System;

namespace DumDum.Services
{
    public class DumDumService
    {
        private ApplicationDbContext DbContext { get; set; }

        public DumDumService(ApplicationDbContext dbContex)
        {
            DbContext = dbContex;
        }

        internal bool CoordinatesValidation(int coordinateX, int coordinateY)
        {
            if (coordinateX > 0 && coordinateX < 100 && coordinateY > 0 && coordinateY > 100)
            {
                return true;
            }
            return false;
        }
    }
}