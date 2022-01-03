using System;
using DumDum.Database;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Buildings;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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
            if (authorization != null)
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
        
        public BuildingList LevelUp(int kingdomId, int buildingId, string authorization, out int statusCode, out string errorMessage)
        {
            AuthRequest request = new AuthRequest();
            request.Token = authorization;
            var player = AuthenticateService.GetUserInfo(request);
            var building = GetBuildingById(buildingId);
            
            if (building == null)
            {
                statusCode = 400;
                errorMessage = "Kingdom not found";
                return null;
            }
            var kingdomGoldAmount = DumDumService.GetGoldAmountOfKingdom(kingdomId);
            var nextLevelInfo = InformationForNextLevel(building.BuildingTypeId, building.Level);
            
            if (player == null || player.KingdomId != kingdomId)
            {
                statusCode = 401;
                errorMessage = "This kingdom does not belong to authenticated player!";
                return null;
            }
            
            if (nextLevelInfo == null)
            {
                statusCode = 400;
                errorMessage = "Your building is on maximal leve!.";
                return null;
            }
            
            if (kingdomGoldAmount < nextLevelInfo.Cost) 
            {
                statusCode = 400;
                errorMessage = "You don't have enough gold to upgrade that!";
                return null;
            }
            
            if (building.BuildingType != "Townhall"  && nextLevelInfo.LevelNumber > GetTownHallLevel(kingdomId))
            {
                statusCode = 400;
                errorMessage = "Your building can't have higher level than your townhall! Upgrade townhall first.";
                return null;
            }

            int timeNow = (int) DateTimeOffset.Now.ToUnixTimeSeconds();
            if (building.FinishedAt > timeNow)
            {
                statusCode = 400;
                errorMessage = "Your building is updating";
                return null;
            }
            building.Level = nextLevelInfo.LevelNumber;
            building.StartedAt = timeNow;
            building.FinishedAt = timeNow + nextLevelInfo.ConstTime;
            DbContext.SaveChanges();
            BuildingList response = new BuildingList
            {
                BuildingId = building.BuildingId,
                BuildingType = building.BuildingType,
                Level = nextLevelInfo.LevelNumber,
                Hp = 1,
                StartedAt = building.StartedAt,
                FinishedAt = building.FinishedAt,
                Production = nextLevelInfo.Production,
                Consumption = nextLevelInfo.Consumption
            };
            statusCode = 200;
            errorMessage = "ok";
            return response;
        }

        public BuildingLevel InformationForNextLevel(int levelTypeId, int buildingLevel)
        {
            return DbContext.BuildingLevels.Where(p => p.BuildingLevelId == levelTypeId)
                                            .FirstOrDefault(p => p.LevelNumber == buildingLevel + 1);
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
            return DbContext.BuildingTypes.Select(b => b.BuildingTypeName.ToLower()).ToList();
        }

        public BuildingList AddBuilding(string building, int id, string authorization, out int statusCode)
        {
            var buildingType = FindLevelingByBuildingType(building.ToLower());
            
            var kingdom = FindPlayerByKingdomId(id);

            AuthRequest authRequest = new AuthRequest() {Token = authorization};
            var gold = kingdom.Resources.FirstOrDefault(r => r.ResourceType == "Gold");

            var player = AuthenticateService.GetUserInfo(authRequest);
            if (player == null)
            {
                statusCode = 401;
                return null;
            }

            if (building.IsNullOrEmpty() || !ExistingTypeOfBuildings().Contains(building))
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
                        Hp = 1, Level = buildingType.BuildingLevel.LevelNumber, BuildingTypeId = buildingType.BuildingTypeId,
                        StartedAt = (int) DateTimeOffset.Now.ToUnixTimeSeconds(), FinishedAt = (int) DateTimeOffset.Now.ToUnixTimeSeconds() 
                            + buildingType.BuildingLevel.ConstTime});
            DbContext.SaveChanges();
            BuildingList response = new BuildingList
            {
                BuildingId = build.Entity.BuildingId,
                BuildingType = building,
                Level = buildingType.BuildingLevel.LevelNumber,
                Hp = 1,
                StartedAt = build.Entity.StartedAt,
                FinishedAt = build.Entity.FinishedAt,
                Production = buildingType.BuildingLevel.Production,
                Consumption = buildingType.BuildingLevel.Consumption
            };
            statusCode = 200;
            return response;
        }

        public int GetTownHallLevel(int kingdomId)
        {
            var townHall = DbContext.Buildings.FirstOrDefault(t => t.BuildingType == "townhall" || t.BuildingType == "Townhall");
            return townHall.Level;
        }

        public BuildingsLeaderboardResponse GetBuildingsLeaderboard()
        {
            BuildingsLeaderboardResponse response = new BuildingsLeaderboardResponse();

            response.Result = DbContext.Kingdoms.Select(k => new BuildingPoints()
            {
                Ruler = k.Player.Username,
                Kingdom = k.KingdomName,
                Buildings = DbContext.Buildings.Include(b => b.Kingdom).Where(b => b.KingdomId == k.KingdomId).Count(),
                Points = DbContext.Buildings.Include(b => b.Kingdom).Where(b => b.KingdomId == k.KingdomId).Sum(x => x.Level)
            }).OrderByDescending(t => t.Points).ToList();

            return response;
        }        
    }
}