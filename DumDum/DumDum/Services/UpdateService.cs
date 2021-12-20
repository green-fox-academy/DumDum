using System;
using System.Collections.Generic;
using System.Linq;
using DumDum.Database;
using DumDum.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DumDum.Services
{
    public class UpdateService
    {
        public ApplicationDbContext DbContext  { get; set; }

        public UpdateService(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }
        public List<Building> GetBuildingListFromDatabase()
        {
            return DbContext.Buildings.ToList();
        }
        
        public void IsActiveChecker(List<Building> buildings)
        {
            for (int i = 0; i < buildings.Count; i++)
            {
                if (buildings[i].FinishedAt < (int)DateTimeOffset.Now.ToUnixTimeSeconds())
                {
                    buildings[i].IsActive = true;
                }
            }
        }
    }
}