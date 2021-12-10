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

        public List<TroopsResponse> GetTroops(int kingdomId)
        {
            List<TroopsResponse> troops = DbContext.Troops.Where(t => t.KingdomId == kingdomId).
                Select(t => new TroopsResponse()
                {
                    TroopId = t.TroopId,
                    TroopType = t.TroopType,
                    Level = t.Level,
                    HP = t.HP,
                    Attack = t.Attack,
                    Defence = t.Defence,
                    StartedAt = t.StartedAt,
                    FinishedAt = t.FinishedAt
                }).ToList();
            if (troops != null)
            {
                return troops;
            }
            return new List<TroopsResponse>();
        }

        internal GetTroopsResponse ListTroops(string authorization, int kingdomId, out int statusCode)
        {
            var response = new GetTroopsResponse();
            if (authorization != null && kingdomId != null)
            {
                AuthRequest request = new AuthRequest();
                request.Token = authorization.Remove(0, 7);
                var player = AuthenticateService.GetUserInfo(request);

                if (player != null && player.KingdomId == kingdomId)
                {
                    response.KingdomId = player.KingdomId;
                    response.KingdomName = player.KingdomName;
                    response.Ruler = player.Ruler;
                    response.Location = new Location();
                    response.Location.CoordinateX = DumDumService.GetKingdomById(player.KingdomId).CoordinateX;
                    response.Location.CoordinateY = DumDumService.GetKingdomById(player.KingdomId).CoordinateY;
                    response.Troops = GetTroops(player.KingdomId);

                    statusCode = 200;
                    return response;
                }
            }
            statusCode = 400;
            response.Error = "This kingdom does not belong to authenticated player";
            return  response;
        }
    }
}
