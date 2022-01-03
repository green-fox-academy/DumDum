using System.Collections.Generic;
using System.Linq;
using DumDum.Database;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Kingdom;
using DumDum.Models.JsonEntities.Resources;

namespace DumDum.Services
{
    public class ResourceService
    {
        private ApplicationDbContext DbContext { get; set; }
        public AuthenticateService AuthenticateService { get; set; }
        public DumDumService DumDumService { get; set; }

        public ResourceService(ApplicationDbContext dbContext, AuthenticateService authenticateService,
            DumDumService dumDumService)
        {
            DbContext = dbContext;
            AuthenticateService = authenticateService;
            DumDumService = dumDumService;
        }

        public List<ResourceList> GetResources(int id)
        {
            return DbContext.Resources.Where(r => r.KingdomId == id).Select(r => new ResourceList()
            {
                ResourceType = r.ResourceType,
                Amount = r.Amount,
                Generation = r.Generation,
                UpdatedAt = r.UpdatedAt
            }).ToList();
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