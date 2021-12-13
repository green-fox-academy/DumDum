using DumDum.Database;
using DumDum.Models.JsonEntities;

namespace DumDum.Services
{
    public class DetailService
    {
        private ApplicationDbContext DbContext { get; set; }
        public BuildingService BuildingService { get; set; }
        public ResourceService ResourceService { get; set; }
        public TroopService TroopService { get; set; }
        public AuthenticateService AuthenticateService { get; set; }
        

        public DetailService(ApplicationDbContext dbContex, BuildingService buildingService, ResourceService resourceService, TroopService troopService, AuthenticateService authenticateService)
        {
            DbContext = dbContex;
            ResourceService = resourceService;
            BuildingService = buildingService;
            TroopService = troopService;
            AuthenticateService = authenticateService;
        }
        
        public KingdomDetailsResponse KingdomInformation(int kingdomId, string authorization, out int statusCode)
        {
            KingdomDetailsResponse kingdomDetailsResponse = new KingdomDetailsResponse();
            AuthRequest response = new AuthRequest();
            response.Token = authorization;
            var player = AuthenticateService.GetUserInfo(response);
            if (player != null)
            {
                kingdomDetailsResponse.Kingdom = BuildingService.GetKingdom(kingdomId);
                kingdomDetailsResponse.Resources = ResourceService.GetResources(kingdomId);
                kingdomDetailsResponse.Buildings = BuildingService.GetBuildings(kingdomId);
                kingdomDetailsResponse.Troops = TroopService.GetTroops(kingdomId);
                statusCode = 200;
                return kingdomDetailsResponse;
            }
            statusCode = 401;
            return kingdomDetailsResponse;
        }
    }
}