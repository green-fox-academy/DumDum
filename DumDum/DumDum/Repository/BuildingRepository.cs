using DumDum.Database;
using DumDum.Interfaces;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Buildings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DumDum.Repository
{
    public class BuildingRepository : Repository<Building>, IBuildingRepository
    {
        public BuildingRepository(ApplicationDbContext context) : base(context)
        {
        }

        public List<BuildingList> GetBuildings(int Id)
        {
            return DbContext.Buildings.Where(b => b.KingdomId == Id).Select(b => new BuildingList()
            {
                BuildingId = b.BuildingId,
                BuildingType = b.BuildingType,
                Level = b.Level,
                StartedAt = b.StartedAt,
                FinishedAt = b.FinishedAt
            }).ToList();
        }

        public Building AddBuilding(string building, Kingdom kingdom, BuildingType buildingType)
        {
            return DbContext.Buildings.Add(new Building()
            {
                BuildingType = building,
                KingdomId = kingdom.KingdomId,
                Kingdom = kingdom,
                Hp = 1,
                Level = buildingType.BuildingLevel.LevelNumber,
                BuildingTypeId = buildingType.BuildingTypeId,
                StartedAt = (int)DateTimeOffset.Now.ToUnixTimeSeconds(),
                FinishedAt = (int)DateTimeOffset.Now.ToUnixTimeSeconds()
                           + buildingType.BuildingLevel.ConstTime
            }).Entity;
        }

        public double GetAllBuildingsConsumptionInKingdom(Kingdom kingdom)
        {
            return DbContext.Buildings.Include(b => b.Kingdom).Where(b => b.KingdomId == kingdom.KingdomId).Sum(x => x.Level);
        }

        public List<Building> GetListOfBuildingsByType(int kingdomId, int buildingTypeId)
        {
            return DbContext.Buildings.Where(b => b.KingdomId == kingdomId && b.BuildingTypeId == buildingTypeId).ToList();
        }
        
    }
}
