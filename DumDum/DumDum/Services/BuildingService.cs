using System;
using DumDum.Database;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Buildings;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Kingdom;
using Microsoft.IdentityModel.Tokens;
using DumDum.Interfaces;

namespace DumDum.Services
{
    public class BuildingService
    {
        private AuthenticateService AuthenticateService { get; set; }
        private DumDumService DumDumService { get; set; }
        private IUnitOfWork UnitOfWork { get; set; }

        public BuildingService(AuthenticateService authService,
            DumDumService dumService, IUnitOfWork unitOfWork)
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
            return new KingdomResponse(DumDumService.GetKingdomById(id), DumDumService.GetPlayerById(kingdom.PlayerId),
                DumDumService.AddLocations(kingdom));
        }

        public BuildingResponse ListBuildings(string authorization, int kingdomId, out int statusCode)
        {
            if (authorization != null && kingdomId != null)
            {
                
                AuthRequest request = new AuthRequest();
                request.Token = authorization;
                var player = AuthenticateService.GetUserInfo(request);
                if (player != null && player.KingdomId == kingdomId)
                {
                    statusCode = 200;
                    return new BuildingResponse(GetKingdom(kingdomId), GetBuildings(kingdomId));
                }
            }
            statusCode = 401;
            return null;
        }

        private Building GetBuildingById(int buildingId)
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
            BuildingList response = new BuildingList(building, nextLevelInfo);

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
            BuildingList response = new BuildingList(build, buildingType);
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