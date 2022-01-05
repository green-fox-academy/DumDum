using System;
using System.Threading.Tasks;
using DumDum.Database;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Kingdom;

namespace DumDum.Services
{
    public class DetailService
    {
        private ApplicationDbContext DbContext { get; set; }
        public BuildingService BuildingService { get; set; }
        public ResourceService ResourceService { get; set; }
        public TroopService TroopService { get; set; }
        public AuthenticateService AuthenticateService { get; set; }
        

        public DetailService(ApplicationDbContext dbContext, BuildingService buildingService,
            ResourceService resourceService, TroopService troopService, AuthenticateService authenticateService)
        {
            DbContext = dbContext;
            ResourceService = resourceService;
            BuildingService = buildingService;
            TroopService = troopService;
            AuthenticateService = authenticateService;
        }
        
        public async Task<(KingdomDetailsResponse, int)> KingdomInformation(int kingdomId, string authorization)
        {
            AuthRequest response = new AuthRequest();
            response.Token = authorization;
            var player = AuthenticateService.GetUserInfo(response);
            if (player != null)
            {
                KingdomDetailsResponse kingdomDetailsResponse = new KingdomDetailsResponse(BuildingService.GetKingdom(kingdomId), ResourceService.GetResources(kingdomId), BuildingService.GetBuildings(kingdomId), TroopService.GetTroops(kingdomId));
                kingdomDetailsResponse.Kingdom = BuildingService.GetKingdom(kingdomId);
                kingdomDetailsResponse.Resources = ResourceService.GetResources(kingdomId);
                kingdomDetailsResponse.Buildings = BuildingService.GetBuildings(kingdomId);
                kingdomDetailsResponse.Troops = TroopService.GetTroops(kingdomId);
                return (kingdomDetailsResponse, 200);
            }
            return (null, 401);
        }
    }
}