using System.Collections.Generic;
using System.Linq;
using DumDum.Database;
using DumDum.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DumDum.Services
{
    public class ResourceService
    {
        private ApplicationDbContext DbContext { get; set; }

        public ResourceService(ApplicationDbContext dbContex)
        {
            DbContext = dbContex;
        }

        public List<Resource> GetResources(int id)
        {
            return DbContext.Resources.Where(r => r.KingdomId == id).ToList();
        }

        public Location AddLocations(Kingdom kingdom)
        {
            return new Location() {CoordinateX = kingdom.CoordinateX, CoordinateY = kingdom.CoordinateY};
        }
    }
}
