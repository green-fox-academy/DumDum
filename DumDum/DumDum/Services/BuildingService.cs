using DumDum.Interfaces;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Buildings;
using DumDum.Models.JsonEntities.Kingdom;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DumDum.Services
{
    public class BuildingService : IBuildingService
    {
        private IAuthenticateService AuthenticateService { get; set; }
        private IDumDumService DumDumService { get; set; }
        private IUnitOfWork UnitOfWork { get; set; }

        public BuildingService(IAuthenticateService authService,
            IDumDumService dumService, IUnitOfWork unitOfWork)
        {
            AuthenticateService = authService;
            DumDumService = dumService;
            UnitOfWork = unitOfWork;
        }

        public List<BuildingList> GetBuildings(int id)
        {
            return UnitOfWork.Buildings.GetBuildings(id);
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

        public Building GetBuildingById(int buildingId)
        {
            return UnitOfWork.Buildings.Find(b => b.BuildingId == buildingId).FirstOrDefault();
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
            UnitOfWork.Complete();
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
            return UnitOfWork.BuildingLevels.Find(p => p.BuildingLevelId == levelTypeId)
                                            .FirstOrDefault(p => p.LevelNumber == buildingLevel + 1);
        }

        public Kingdom FindPlayerByKingdomId(int id)
        {
            return UnitOfWork.Kingdoms.FindPlayerByKingdomId(id);
        }

        public BuildingType FindLevelingByBuildingType(string buildingType)
        {
            return UnitOfWork.BuildingTypes.FindLevelingByBuildingType(buildingType);
        }

        public List<string> ExistingTypeOfBuildings()
        {
            return UnitOfWork.BuildingTypes.ExistingTypeOfBuildings();
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
            var build = UnitOfWork.Buildings.AddBuilding(building, kingdom, buildingType);
            UnitOfWork.Complete();
            BuildingList response = new BuildingList
            {
                BuildingId = build.BuildingId,
                BuildingType = building,
                Level = buildingType.BuildingLevel.LevelNumber,
                Hp = 1,
                StartedAt = build.StartedAt,
                FinishedAt = build.FinishedAt,
                Production = buildingType.BuildingLevel.Production,
                Consumption = buildingType.BuildingLevel.Consumption
            };
            statusCode = 200;
            return response;
        }

        public int GetTownHallLevel(int kingdomId)
        {
            return UnitOfWork.Buildings.Find(t => t.BuildingType == "townhall" || t.BuildingType == "Townhall").FirstOrDefault().Level;
        }

        public BuildingsLeaderboardResponse GetBuildingsLeaderboard()
        {
            BuildingsLeaderboardResponse response = new BuildingsLeaderboardResponse();
            response.Result = UnitOfWork.Kingdoms.GetListBuildingPoints();
            return response;
        }        
    }
}