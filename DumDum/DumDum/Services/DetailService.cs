using DumDum.Interfaces;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Kingdom;

namespace DumDum.Services
{
    public class DetailService : IDetailService
    {
        public BuildingService BuildingService { get; set; }
        public ResourceService ResourceService { get; set; }
        public TroopService TroopService { get; set; }
        public IAuthenticateService AuthenticateService { get; set; }
        public DetailService( BuildingService buildingService,
            ResourceService resourceService, TroopService troopService, IAuthenticateService authenticateService)
        {
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