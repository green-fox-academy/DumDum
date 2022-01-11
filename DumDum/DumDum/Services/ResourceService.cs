using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DumDum.Database;
using DumDum.Interfaces;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Kingdom;
using DumDum.Models.JsonEntities.Resources;

namespace DumDum.Services
{
    public class ResourceService
    {
        public AuthenticateService AuthenticateService { get; set; }
        public DumDumService DumDumService { get; set; }
        private IUnitOfWork UnitOfWork { get; set; }

        public ResourceService(AuthenticateService authenticateService, DumDumService dumDumService, IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
            AuthenticateService = authenticateService;
            DumDumService = dumDumService;
            UnitOfWork = unitOfWork;
        }
        public List<ResourceList> GetResources(int kingdomId)
        {
            return UnitOfWork.Resources.GetResources(kingdomId).Result;
        }

        public Location AddLocations(Kingdom kingdom)
        {
            return new Location() {CoordinateX = kingdom.CoordinateX, CoordinateY = kingdom.CoordinateY};
        }

        public async Task<(ResourceResponse, int)> ResourceLogic(int id, string authorization)
        {
            if (authorization != null)
            {
                AuthRequest request = new AuthRequest();
                request.Token = authorization.Remove(0, 7);
                var player = await AuthenticateService.GetUserInfo(request);

                if (player != null && player.KingdomId == id)
                {
                    var kingdom = DumDumService.GetKingdomById(id);
                    if (kingdom is null)
                    {
                        return (null, 404);
                    }
                    var locations = AddLocations(kingdom);
                    var resources = GetResources(id);
                    var resResp = new ResourceResponse(new KingdomResponse(kingdom), resources);
                    return (resResp, 200);

                }
            }
            return (new ResourceResponse(), 401);
        }
    }
}