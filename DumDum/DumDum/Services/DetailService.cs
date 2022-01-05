using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Kingdom;

namespace DumDum.Services
{
    public class DetailService
    {
        public BuildingService BuildingService { get; set; }
        public ResourceService ResourceService { get; set; }
        public TroopService TroopService { get; set; }
        public AuthenticateService AuthenticateService { get; set; }
        public DetailService( BuildingService buildingService,
            ResourceService resourceService, TroopService troopService, AuthenticateService authenticateService)
        {
            ResourceService = resourceService;
            BuildingService = buildingService;
            TroopService = troopService;
            AuthenticateService = authenticateService;
        }
        
        public KingdomDetailsResponse KingdomInformation(int kingdomId, string authorization, out int statusCode)
        {
            AuthRequest response = new AuthRequest();
            response.Token = authorization;
            var player = AuthenticateService.GetUserInfo(response);
            if (player != null)
            {
                KingdomDetailsResponse kingdomDetailsResponse = new KingdomDetailsResponse(BuildingService
                    .GetKingdom(kingdomId), ResourceService
                    .GetResources(kingdomId), BuildingService.GetBuildings(kingdomId), TroopService
                    .GetTroops(kingdomId));
                statusCode = 200;
                return kingdomDetailsResponse;
            }
            statusCode = 401;
            return null;
        }
    }
}