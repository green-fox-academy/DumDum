using DumDum.Interfaces;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Kingdom;
using DumDum.Models.JsonEntities.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;
using DumDum.Interfaces.IRepositories;
using DumDum.Interfaces.IServices;

namespace DumDum.Services
{
    public class ResourceService : IResourceService
    {
        private IAuthenticateService AuthenticateService { get; set; }
        private IDumDumService DumDumService { get; set; }
        private IUnitOfWork UnitOfWork { get; set; }

        public ResourceService(IAuthenticateService authenticateService, IDumDumService dumDumService, IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
            AuthenticateService = authenticateService;
            DumDumService = dumDumService;
        }
        public async Task<List<ResourceList>> GetResources(int kingdomId)
        {
            return await UnitOfWork.Resources.GetResources(kingdomId);
        }

        public async Task<Location> AddLocations(Kingdom kingdom)
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
                    var kingdom = await DumDumService.GetKingdomById(id);
                    if (kingdom is null)
                    {
                        return (null, 404);
                    }
                    var locations = AddLocations(kingdom);
                    var resources = await GetResources(id);
                    var resResp = new ResourceResponse(new KingdomResponse(kingdom), resources);
                    return (resResp, 200);

                }
            }
            return (new ResourceResponse(), 401);
        }
    }
}