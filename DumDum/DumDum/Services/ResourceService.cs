using System.Collections.Generic;
using System.Linq;
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
            return UnitOfWork.Resources.GetResources(kingdomId);
            return DbContext.Resources.Where(r => r.KingdomId == id).Select(r => new ResourceList(r)).ToList();
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
                        Kingdom = new KingdomResponse(kingdom),
                        Resources = resources
                    };
                }
            }

            statusCode = 401;
            return new ResourceResponse();
        }
    }
}