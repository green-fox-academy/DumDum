using System;
using System.Collections.Generic;
using System.Linq;
using DumDum.Database;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using Microsoft.EntityFrameworkCore;

namespace DumDum.Services
{
    public class ResourceService
    {
        private ApplicationDbContext DbContext { get; set; }
        public AuthenticateService AuthenticateService { get; set; }
        public DumDumService DumDumService { get; set; }

        public ResourceService(ApplicationDbContext dbContex, AuthenticateService authenticateService, DumDumService dumDumService)
        {
            DbContext = dbContex;
            AuthenticateService = authenticateService;
            DumDumService = dumDumService;
        }

        public List<Resource> GetResources(int id)
        {
            return DbContext.Resources.Where(r => r.KingdomId == id).ToList();
        }

        public Location AddLocations(Kingdom kingdom)
        {
            return new Location() {CoordinateX = kingdom.CoordinateX, CoordinateY = kingdom.CoordinateY};
        }
        
        public Object ResourceLogic(int id, string authorization, out int statusCode)
        {
            AuthRequest request = new AuthRequest();
            request.Token = authorization.Remove(0, 7);
            var player = AuthenticateService.GetUserInfo(request);

            if (player != null && player.KingdomId == id)
            {
                var kingdom = DumDumService.GetKingdomById(id);
                var locations = AddLocations(kingdom);
                var resources = GetResources(id);
                statusCode = 200;
                return new 
                {
                    KingdomId = kingdom.KingdomId, 
                    KingdomName = kingdom.KingdomName,
                    Ruler = player.Ruler, 
                    Location = locations,
                    Resources = resources.Select(x=> new{Type=x.ResourceType,Amount = x.Amount,Generation = x.Generation,UpdatedAt = x.UpdatedAt}).ToList()
                };
            }

            statusCode = 401;
            return new ResourceResponse();
        }
    }
}
