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
            return response;
        }

        internal List<TroopsResponse> CreateTroops(string authorization, TroopCreationRequest troopCreationReq, int kingdomId, out int statusCode)
        {
            var player = CheckToken(authorization);
            var goldAmount = DumDumService.GetGoldAmountOfKingdom(kingdomId);
            var createdTroops = new List<TroopsResponse>();

            if (player != null && player.KingdomId == kingdomId)
            {
                switch (troopCreationReq.Type.ToLower())
                {
                    case "axeman":
                        var axeman = new Axeman();
                        if (goldAmount < axeman.Cost * troopCreationReq.Quantity)
                        {
                            statusCode = 400;
                        }
                        for (int i = 0; i < troopCreationReq.Quantity; i++)
                        {
                            var newAxeman = new Axeman();
                            newAxeman.KingdomId = kingdomId;
                            DbContext.Troops.Add(newAxeman);
                            DbContext.SaveChanges();
                            DumDumService.TakeGold(kingdomId, axeman.Cost);
                            createdTroops.Add(new TroopsResponse()
                            {
                                TroopId = newAxeman.TroopId,
                                TroopType = newAxeman.TroopType,
                                Level = newAxeman.Level,
                                HP = newAxeman.HP,
                                Attack = newAxeman.Attack,
                                Defence = newAxeman.Defence,
                                StartedAt = newAxeman.StartedAt,
                                FinishedAt = newAxeman.FinishedAt
                            });
                        }
                        statusCode = 200;
                        return createdTroops;
                        break;
                }

            }

            statusCode = 401;
            return new List<TroopsResponse>();
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
