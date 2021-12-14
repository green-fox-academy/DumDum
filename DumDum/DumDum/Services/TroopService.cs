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
        private AuthenticateService AuthenticateService { get; set; }
        private DumDumService DumDumService { get; set; }
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
            if (authorization != "")
            {
                AuthRequest request = new AuthRequest();
                request.Token = authorization.Remove(0, 7);
                var player = AuthenticateService.GetUserInfo(request);
                

                if (player != null && player.KingdomId == kingdomId)
                {
                    var plyersKingdom = DumDumService.GetKingdomById(player.KingdomId);
                    response.Kingdom = new KingdomResponse
                    {
                        KingdomId = player.KingdomId,
                        KingdomName = player.KingdomName,
                        Ruler = player.Ruler,
                        Location = new Location() { CoordinateX = plyersKingdom.CoordinateX, CoordinateY = plyersKingdom.CoordinateY }
                    };
                    response.Troops = GetTroops(player.KingdomId);

                    statusCode = 200;
                    return response;
                }
            }
            statusCode = 401;
            return  response;
        }

        internal TroopsResponse CreateTroops(string authorization, int kingdomId, out int statusCode)
        {
            var player = CheckToken(authorization);

            if (player != null)
            {
                
            }
        }

        internal AuthResponse CheckToken(string authorization)
        {
            var player = new AuthResponse();
            if (authorization != "")
            {
                AuthRequest request = new AuthRequest();
                request.Token = authorization.Remove(0, 7);
                player = AuthenticateService.GetUserInfo(request);
                return player;
            }
            return player;
        }   
    }
}
