using System;
using DumDum.Database;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Buildings;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Kingdom;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DumDum.Services
{
    public class BuildingService
    {
        private ApplicationDbContext DbContext { get; set; }
        private AuthenticateService AuthenticateService { get; set; }
        private DumDumService DumDumService { get; set; }

        public BuildingService(ApplicationDbContext dbContext, AuthenticateService authService,
            DumDumService dumService)
        {
            DbContext = dbContext;
            AuthenticateService = authService;
            DumDumService = dumService;
        }

        public List<BuildingList> GetBuildings(int id)
        {
            return DbContext.Buildings.Where(b => b.KingdomId == id).Select(b => new BuildingList()
            {
                BuildingId = b.BuildingId,
                BuildingType = b.BuildingType,
                Level = b.Level,
                StartedAt = b.StartedAt,
                FinishedAt = b.FinishedAt
            }).ToList();
        }

        public KingdomResponse GetKingdom(int id)
        {
            var kingdom = DumDumService.GetKingdomById(id);
            var player = DumDumService.GetPlayerById(kingdom.PlayerId);
            var locations = DumDumService.AddLocations(kingdom);
            return new KingdomResponse()
            {
                KingdomId = kingdom.KingdomId,
                KingdomName = kingdom.KingdomName,
                Ruler = player.Username,
                Population = 0,
                Location = locations,
            };
        }

        public BuildingResponse ListBuildings(string authorization, int kingdomId, out int statusCode)
        {
            var response = new BuildingResponse();
            if (authorization != null && kingdomId != null)
            {
                AuthRequest request = new AuthRequest();
                request.Token = authorization;
                var player = AuthenticateService.GetUserInfo(request);
                if (player != null && player.KingdomId == kingdomId)
                {
                    response.Kingdom = GetKingdom(kingdomId);
                    response.Buildings = GetBuildings(kingdomId);
                    statusCode = 200;
                    return response;
                }
            }

            statusCode = 401;
            return response;
        }

        private Building GetBuildingById(int buildingId)
        {
            return DbContext.Buildings.FirstOrDefault(b => b.BuildingId == buildingId);
        }
        
        public BuildingList LevelUp(int kingdomId, int buildingId, out int statusCode, string authorization, out string exception)
        {
            AuthRequest request = new AuthRequest();
            request.Token = authorization;
            var player = AuthenticateService.GetUserInfo(request);
            BuildingList response = new BuildingList();
            var building = GetBuildingById(buildingId);
            var kingdomGoldAmount = DumDumService.GetGoldAmountOfKingdom(kingdomId);
            var nextLevelInfo = InformationForNextLevel(building.BuildingTypeId, building.Level);
            

            if (player == null)
            {
                statusCode = 401;
                exception = "authentication";
                return null;
            }
            if (building == null)
            {
                statusCode = 400;
                exception = "notBuilding";
                return null;
            }

            if (nextLevelInfo == null)
            {
                statusCode = 400;
                exception = "maxLevel";
                return null;
            }
            if (kingdomGoldAmount < nextLevelInfo.Cost) 
            {
                statusCode = 400;
                exception = "enoughGold";
                return null;
            }

            if ( nextLevelInfo.LevelNumber > GetTownHallLevel(kingdomId))
            {
                statusCode = 400;
                exception = "townHall";
                return null;
            }
            
            var editedBuilding = DbContext.Buildings.FirstOrDefault(b => b.BuildingId == buildingId);
            editedBuilding.Level = nextLevelInfo.LevelNumber;
            DbContext.SaveChanges();
            
            response.BuildingId = building.BuildingId;
            response.BuildingType = building.BuildingType;
            response.Level = editedBuilding.Level;
            response.Hp = 1;
            response.StartedAt = 112;
            response.FinishedAt = 123;
            response.Production = nextLevelInfo.Production;
            response.Consumption = nextLevelInfo.Consumption;
            statusCode = 200;
            exception = "ok";
            return response;
        }

        public BuildingLevel InformationForNextLevel(int levelTypeId, int buildingLevel)
        {
            return DbContext.BuildingLevels.Where(p => p.BuildingLevelId == levelTypeId).FirstOrDefault(p => p.LevelNumber == buildingLevel + 1);
        }

        public Kingdom FindPlayerByKingdomId(int id)
        {
            var kingdom = DbContext.Kingdoms.Include(p => p.Player)
                .Include(r => r.Resources)
                .FirstOrDefault(k => k.KingdomId == id);
            return kingdom;
        }

        public BuildingType FindLevelingByBuildingType(string buildingType)
        {
            var level = DbContext.BuildingTypes.Include(b => b.BuildingLevels)
                .FirstOrDefault(p => p.BuildingTypeName == buildingType);
            return level;
        }

        public List<string> ExistingTypeOfBuildings()
        {
            return DbContext.BuildingTypes.Select(b => b.BuildingTypeName).ToList();
        }

        public BuildingList AddBuilding(string building, int id, string authorization, out int statusCode)
        {
            var buildingType = FindLevelingByBuildingType(building.ToLower());
            BuildingList response = new BuildingList();
            var kingdom = FindPlayerByKingdomId(id);

            AuthRequest authRequest = new AuthRequest() {Token = authorization};
            var gold = kingdom.Resources.FirstOrDefault(r => r.ResourceType == "Gold");

            var player = AuthenticateService.GetUserInfo(authRequest);
            if (player == null)
            {
                statusCode = 401;
            }

            if (building.IsNullOrEmpty())
            {
                statusCode = 406;
                return null;
            }
            
            if (!ExistingTypeOfBuildings().Contains(building.First().ToString().ToUpper() + building.Substring(1)))
            {
                statusCode = 406;
                return null;
            }

            if (gold?.Amount < buildingType.BuildingLevel.Cost)
            {
                statusCode = 400;
                return null;
            }
            var build = DbContext.Buildings.Add(new Building()
                        {BuildingType = building, KingdomId = kingdom.KingdomId, Kingdom = kingdom,
                        Hp = 1, Level = buildingType.BuildingLevel.LevelNumber, BuildingTypeId = buildingType.BuildingTypeId});
            DbContext.SaveChanges();
            response.BuildingId = build.Entity.BuildingId;
            response.BuildingType = building;
            response.Level = buildingType.BuildingLevel.LevelNumber;
            response.Hp = 1;
            response.StartedAt = 112;
            response.FinishedAt = 123;
            response.Production = buildingType.BuildingLevel.Production;
            response.Consumption = buildingType.BuildingLevel.Consumption;
            statusCode = 200;
            return response;
        }

        public int GetTownHallLevel(int kingdomId)
        {
            var townHall = DbContext.Buildings.FirstOrDefault(t => t.BuildingType == "townhall" || t.BuildingType == "Townhall");
            return townHall.Level;
        }
    }
}