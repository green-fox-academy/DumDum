using DumDum.Database;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DumDum.Services
{
    public class TroopService
    {
        private ApplicationDbContext DbContext { get; set; }
        public AuthenticateService AuthenticateService { get; set; }
        public DumDumService DumDumService { get; set; }
        public TroopService(ApplicationDbContext dbContext, AuthenticateService authService, DumDumService dumService)
        {
            DbContext = dbContext;
            AuthenticateService = authService;
            DumDumService = dumService;
        }

        public List<Troop> GetTroops(int kingdomId)
        {
            List<Troop> troops = DbContext.Troops.Where(r => r.KingdomId == kingdomId).ToList();
            if (troops != null)
            {
                return troops;
            }
            return new List<Troop>();
        }

        internal TroopResponse ListTroops(string authorization, out int statusCode)
        {
            if (authorization != null)
            {
                AuthRequest request = new AuthRequest();
                request.Token = authorization.Remove(0, 7);
                var player = AuthenticateService.GetUserInfo(request);

                var response = new TroopResponse();
                response.KingdomId = player.KingdomId;
                response.KingdomName = player.KingdomName;
                response.Location = new Location();
                response.Location.CoordinateX = DumDumService.GetKingdomById(player.KingdomId).CoordinateX;
                response.Location.CoordinateY = DumDumService.GetKingdomById(player.KingdomId).CoordinateY;
                response.Troops = GetTroops(player.KingdomId);
                if (player != null)
                {
                    statusCode = 200;
                    return response;
                }
            }
            statusCode = 400;
            return new TroopResponse() { Error = "This kingdom does not belong to authenticated player" };
        }
    }
}
