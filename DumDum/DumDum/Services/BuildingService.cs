using DumDum.Interfaces;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Buildings;
using System.Collections.Generic;

namespace DumDum.Services
{
    public class BuildingService
    {
        private AuthenticateService AuthenticateService { get; set; }
        private DumDumService DumDumService { get; set; }
        private IUnitOfWork UnitOfWork { get; set; }

        public BuildingService(AuthenticateService authService, DumDumService dumService, IUnitOfWork unitOfWork)
        {
            AuthenticateService = authService;
            DumDumService = dumService;
            UnitOfWork = unitOfWork;
        }

        public List<BuildingList> GetBuildings(int kingdomId)
        {
            return UnitOfWork.Buildings.GetBuildings(kingdomId);
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
            return UnitOfWork.Buildings.GetById(buildingId);
        }

        public UpgradeBuildingsResponse UpgradeBuildingsLogic(string authorization, int kingdomId, int buildingId,
            out int statusCode)
        {
            AuthRequest request = new AuthRequest();
            request.Token = authorization;
            var player = AuthenticateService.GetUserInfo(request);
            if (player != null && player.KingdomId == kingdomId)
            {
                var building = GetBuildingById(buildingId);
                Levelup(kingdomId, buildingId, out statusCode);
                return new UpgradeBuildingsResponse()
                {
                    Id = building.BuildingId,
                    Type = building.BuildingType,
                    Level = building.Level,
                    Hp = building.Hp,
                    StartedAt = building.StartedAt,
                    FinishedAt = building.FinishedAt
                };
            }

            statusCode = 401;
            return new UpgradeBuildingsResponse();
        }

        public void Levelup(int kingdomId, int buildingId, out int statusCode)
        {
            var code = 0;
            var building = GetBuildingById(buildingId);
            if (building is null)
            {
                var goldAmount = DumDumService.GetGoldAmountOfKingdom(kingdomId);
                switch (building.BuildingType)
                {
                    case "Farm":
                        if (goldAmount < building.Level * 60)
                        {
                            code = 400;
                        }
                        else
                        {
                            code = 200;
                            DumDumService.TakeGold(kingdomId, building.Level * 60);
                            building.Level++;
                        }

                        break;

                    case "Townhall":
                        if (goldAmount < building.Level * 110)
                        {
                            code = 400;
                        }
                        else
                        {
                            code = 200;
                            DumDumService.TakeGold(kingdomId, building.Level * 110);
                            building.Level++;
                        }

                        break;

                    case "Mine":
                        if (goldAmount < building.Level * 80)
                        {
                            code = 400;
                        }
                        else
                        {
                            code = 200;
                            DumDumService.TakeGold(kingdomId, building.Level * 80);
                            building.Level++;
                        }

                        break;

                    case "Barrack":
                        if (goldAmount < building.Level * 120)
                        {
                            code = 400;
                        }
                        else
                        {
                            code = 200;
                            DumDumService.TakeGold(kingdomId, building.Level * 120);
                            building.Level++;
                        }

                        break;

                    case "Academy":
                        if (goldAmount < building.Level * 130)
                        {
                            code = 400;
                        }
                        else
                        {
                            code = 200;
                            DumDumService.TakeGold(kingdomId, building.Level * 130);
                            building.Level++;
                        }

                        break;
                }

                code = 404;
            }

            statusCode = code;
            UnitOfWork.Complete();
        }
    }
}