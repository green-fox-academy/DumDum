using DumDum.Interfaces;
using DumDum.Interfaces.IServices;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Kingdom;
using System.Threading.Tasks;

namespace DumDum.Services
{
    public class DetailService : IDetailService
    {
        private IBuildingService BuildingService { get; set; }
        private IResourceService ResourceService { get; set; }
        private ITroopService TroopService { get; set; }
        private IAuthenticateService AuthenticateService { get; set; }
        public DetailService(IBuildingService buildingService,
            IResourceService resourceService, ITroopService troopService, IAuthenticateService authenticateService)
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
                    await ResourceService.GetResources(kingdomId),
                    BuildingService.GetBuildings(kingdomId),
                    await TroopService.GetTroops(kingdomId));
                return (kingdomDetailsResponse, 200);
            }
            return (null, 401);
        }
    }
}