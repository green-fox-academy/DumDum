using DumDum.Database;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Buildings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DumDum.Interfaces.IRepositories;

namespace DumDum.Repository
{
    public class BuildingRepository : Repository<Building>, IBuildingRepository
    {
        public BuildingRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<BuildingList>> GetBuildings(int Id)
        {
            var buildingList = DbContext.Buildings.Where(b => b.KingdomId == Id).Select(b => new BuildingList(b)).ToList();
            return buildingList;
        }

        public async Task<Building> AddBuilding(string building, Kingdom kingdom, BuildingType buildingType)
        {
            var addBuilding = DbContext.Buildings.Add(new Building()
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
            return addBuilding;
        }

        public async Task<double> GetAllBuildingsConsumptionInKingdom(Kingdom kingdom)
        {
            var number = DbContext.Buildings.Include(b => b.Kingdom).Where(b => b.KingdomId == kingdom.KingdomId).Sum(x => x.Level);
            return number;
        }

        public List<Building> GetNumberOfFarm(int kingdomId)
        {
            return DbContext.Buildings.Where(b => b.KingdomId == kingdomId && b.BuildingType == "Farm").ToList();
        }
        public List<Building> GetNumberOfMines(int kingdomId)
        {
            return DbContext.Buildings.Where(b => b.KingdomId == kingdomId && b.BuildingType == "Mine").ToList();
        }
    }
}
