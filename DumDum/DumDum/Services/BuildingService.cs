using DumDum.Database;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Buildings;
using System.Collections.Generic;
using System.Linq;

namespace DumDum.Services
{
    public class BuildingService
    {
        private ApplicationDbContext DbContext { get; set; }
        private AuthenticateService AuthenticateService { get; set; }
        private DumDumService DumDumService { get; set; }

        public BuildingService(ApplicationDbContext dbContext, AuthenticateService authService, DumDumService dumService)
        {
            DbContext = dbContext;
            AuthenticateService = authService;
            DumDumService = dumService;
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
                if (statusCode == 200)
                {
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

                return new UpgradeBuildingsResponse();
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
                code = 404;
            }
            else
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
            }
            
            statusCode = code;
            DbContext.SaveChanges();
        }
    }
}