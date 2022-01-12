using DumDum.Interfaces;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Kingdom;
using DumDum.Models.JsonEntities.Resources;
using System.Collections.Generic;

namespace DumDum.Services
{
    public class ResourceService : IResourceService
    {
        public IAuthenticateService AuthenticateService { get; set; }
        public IDumDumService DumDumService { get; set; }
        private IUnitOfWork UnitOfWork { get; set; }

        public ResourceService(IAuthenticateService authenticateService, IDumDumService dumDumService, IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
            AuthenticateService = authenticateService;
            DumDumService = dumDumService;
        }

        public List<ResourceList> GetResources(int kingdomId)
        {
            return UnitOfWork.Resources.GetResources(kingdomId);
        }

        public Location AddLocations(Kingdom kingdom)
        {
            return new Location() {CoordinateX = kingdom.CoordinateX, CoordinateY = kingdom.CoordinateY};
        }

        public ResourceResponse ResourceLogic(int id, string authorization, out int statusCode)
        {
            if (authorization != null)
            {
                AuthRequest request = new AuthRequest();
                request.Token = authorization.Remove(0, 7);
                var player = AuthenticateService.GetUserInfo(request);

                if (player != null && player.KingdomId == id)
                {
                    var kingdom = DumDumService.GetKingdomById(id);
                    if (kingdom is null)
                    {
                        statusCode = 404;
                        return new ResourceResponse();
                    }
                    var locations = AddLocations(kingdom);
                    var resources = GetResources(id);
                    statusCode = 200;
                    return new ResourceResponse()
                    {
                        Kingdom = new KingdomResponse()
                        {
                            KingdomId = kingdom.KingdomId,
                            KingdomName = kingdom.KingdomName,
                            Ruler = player.Ruler,
                            Location = locations,
                        },
                        Resources = resources
                    };
                }
            }

            statusCode = 401;
            return new ResourceResponse();
        }
    }
}