using DumDum.Interfaces;
using DumDum.Interfaces.IServices;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Buildings;
using DumDum.Models.JsonEntities.Kingdom;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<List<BuildingList>> GetBuildings(int id)
        {
            return await UnitOfWork.Buildings.GetBuildings(id);
        }

        public async Task<KingdomResponse> GetKingdom(int id)
        {
            var kingdom = await DumDumService.GetKingdomById(id);
            return new KingdomResponse(await DumDumService.GetKingdomById(id), await DumDumService.GetPlayerById(kingdom.PlayerId), await DumDumService.AddLocations(kingdom));
        }

        public async Task<(BuildingResponse, int)> ListBuildings(string authorization, int kingdomId)
        {
            if (authorization != null)
            {
                AuthRequest request = new AuthRequest();
                request.Token = authorization;
                var player = await AuthenticateService.GetUserInfo(request);
                if (player != null && player.KingdomId == kingdomId)
                {
                    var response = new BuildingResponse(GetKingdom(kingdomId), GetBuildings(kingdomId));
                    return (response, 200);
                }
            }
            return (null, 401);
        }

        public async Task<Building> GetBuildingById(int buildingId)
        {
            return UnitOfWork.Buildings.Find(b => b.BuildingId == buildingId).Result.FirstOrDefault();
        }
        
        public async Task<(BuildingList, int, string)> LevelUp(int kingdomId, int buildingId, string authorization)
        {
            AuthRequest request = new AuthRequest();
            request.Token = authorization;
            var player = await AuthenticateService.GetUserInfo(request);
            var building = await GetBuildingById(buildingId);
            
            if (building == null)
            {
                return (null, 400, "Kingdom not found");
            }
            var kingdomGoldAmount = await DumDumService.GetGoldAmountOfKingdom(kingdomId);
            var nextLevelInfo = await InformationForNextLevel(building.BuildingTypeId, building.Level);
            
            if (player == null || player.KingdomId != kingdomId)
            {
                return (null, 401, "This kingdom does not belong to authenticated player!");
            }
            
            if (nextLevelInfo == null)
            {
                return (null, 400, "Your building is on maximal leve!." );
            }
            
            if (kingdomGoldAmount < nextLevelInfo.Cost) 
            {
                return (null, 400, "You don't have enough gold to upgrade that!");
            }
            
            if (building.BuildingType != "Townhall"  && nextLevelInfo.LevelNumber > GetTownHallLevel(kingdomId).Result)
            {
                return (null, 400, "Your building can't have higher level than your townhall! Upgrade townhall first." );
            }

            int timeNow = (int) DateTimeOffset.Now.ToUnixTimeSeconds();
            if (building.FinishedAt > timeNow)
            {
                return (null, 400, "Your building is updating" );
            }
            building.Level = nextLevelInfo.LevelNumber;
            building.StartedAt = timeNow;
            building.FinishedAt = timeNow + nextLevelInfo.ConstTime;
            UnitOfWork.Complete();
            BuildingList response = new BuildingList(building, nextLevelInfo);
            
            return (response, 200, "ok");
        }

        public async Task<BuildingLevel> InformationForNextLevel(int levelTypeId, int buildingLevel)
        {
            return UnitOfWork.BuildingLevels
                .Find(p => p.BuildingLevelId == levelTypeId && p.LevelNumber == buildingLevel + 1).Result
                .FirstOrDefault();
        }

        public async Task<Kingdom> FindPlayerByKingdomId(int id)
        {
            return await UnitOfWork.Kingdoms.FindPlayerByKingdomId(id);
        }

        public async Task<BuildingType> FindLevelingByBuildingType(string buildingType)
        {
            return await UnitOfWork.BuildingTypes.FindLevelingByBuildingType(buildingType);
        }

        public async Task<List<string>> ExistingTypeOfBuildings()
        {
            return await UnitOfWork.BuildingTypes.ExistingTypeOfBuildings();
        }

        public async Task<(BuildingList, int)> AddBuilding(string building, int id, string authorization)
        {
            var buildingType = await FindLevelingByBuildingType(building.ToLower());
            
            var kingdom =  await FindPlayerByKingdomId(id);

            AuthRequest authRequest = new AuthRequest() {Token = authorization};
            var gold = kingdom.Resources.FirstOrDefault(r => r.ResourceType == "Gold");

            var player = AuthenticateService.GetUserInfo(authRequest);
            if (player == null)
            {
                return (null, 401);
            }

            if (building.IsNullOrEmpty() || !ExistingTypeOfBuildings().Result.Contains(building))
            {
                return (null, 406);
            }
            
            if (gold?.Amount < buildingType.BuildingLevel.Cost)
            {
                return (null, 400);
            }
            var build = UnitOfWork.Buildings.AddBuilding(building, kingdom, buildingType);
            UnitOfWork.Complete();
            BuildingList response = new BuildingList(build, buildingType);
            return (response, 200);
        }

        public async Task<int> GetTownHallLevel(int kingdomId)
        {
            return UnitOfWork.Buildings.Find(t => t.BuildingType == "townhall" || t.BuildingType == "Townhall").Result
                                       .FirstOrDefault().Level;
        }

        public async Task<BuildingsLeaderboardResponse> GetBuildingsLeaderboard()
        {
            BuildingsLeaderboardResponse response = new BuildingsLeaderboardResponse();
            response.Result = UnitOfWork.Kingdoms.GetListBuildingPoints().Result;
            return response;
        }        
    }
}