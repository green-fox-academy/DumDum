using System.Threading.Tasks;
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
        
        public async Task<(KingdomDetailsResponse, int)> KingdomInformation(int kingdomId, string authorization)
        {
            AuthRequest response = new AuthRequest();
            response.Token = authorization;
            var player = await AuthenticateService.GetUserInfo(response);
            if (player != null)
            {
                KingdomDetailsResponse kingdomDetailsResponse = new KingdomDetailsResponse(
                    BuildingService.GetKingdom(kingdomId),
                    ResourceService.GetResources(kingdomId),
                    BuildingService.GetBuildings(kingdomId),
                    await TroopService.GetTroops(kingdomId));
                return (kingdomDetailsResponse, 200);
            }
            return (null, 401);
        }
    }
}