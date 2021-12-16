using DumDum.Database;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

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

        internal GetTroopsResponse ListTroops(string authorization, int kingdomId, out int statusCode)
        {
            var response = new GetTroopsResponse();
            var player = CheckToken(authorization);

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

            statusCode = 401;
            return response;
        }

        internal List<TroopsResponse> GetTroops(int kingdomId)
        {
            List<TroopsResponse> troops = DbContext.Troops.Where(t => t.KingdomId == kingdomId).Include(t=>t.TroopType).ToList().
                Select(t => new TroopsResponse()
                {
                    TroopId = t.TroopId,
                    TroopType = t.TroopType.TroopType,
                    Level = t.Level,
                    HP = t.TroopType.TroopLevel.Level,
                    Attack = t.TroopType.TroopLevel.Attack,
                    Defence = t.TroopType.TroopLevel.Defence,
                    StartedAt = t.StartedAt,
                    FinishedAt = t.FinishedAt
                }).ToList();
            if (troops != null)
            {
                return troops;
            }
            return new List<TroopsResponse>();
        }

        internal List<TroopsResponse> CreateTroops(string authorization, TroopCreationRequest troopCreationReq, int kingdomId, out int statusCode)
        {
            var player = CheckToken(authorization);
            var goldAmount = DumDumService.GetGoldAmountOfKingdom(kingdomId);
            var createdTroops = new List<TroopsResponse>();
            var possibleTroops = new List<string> { "spy", "axeman", "phalanx", "knight", "senator" };

            if (troopCreationReq.Type == null || troopCreationReq.Quantity == 0 || !possibleTroops.Contains(troopCreationReq.Type.ToLower()))
            {
                statusCode = 404;
                return new List<TroopsResponse>();
            }
            if (player != null && player.KingdomId == kingdomId)
            {
                int newTroopCost = GetTroopCost(troopCreationReq.Type.ToLower());

                if (goldAmount < newTroopCost * troopCreationReq.Quantity)
                {
                    statusCode = 400;
                    return new List<TroopsResponse>();
                }
                for (int i = 0; i < troopCreationReq.Quantity; i++)
                {
                    var newTroop = CreateNewTroop(troopCreationReq.Type.ToLower(),kingdomId);
                    DbContext.Troops.Add(newTroop);
                    DbContext.SaveChanges();
                    DumDumService.TakeGold(kingdomId, newTroopCost);
                    createdTroops.Add(new TroopsResponse()
                    {
                        TroopId = newTroop.TroopId,
                        TroopType = newTroop.TroopType.TroopType,
                        Level = newTroop.Level,
                        HP = newTroop.TroopType.TroopLevel.HP,
                        Attack = newTroop.TroopType.TroopLevel.Attack,
                        Defence = newTroop.TroopType.TroopLevel.Defence,
                        StartedAt = newTroop.StartedAt,
                        FinishedAt = newTroop.FinishedAt
                    });
                }
                statusCode = 200;
                return createdTroops;
            }
            statusCode = 401;
            return new List<TroopsResponse>();
        }

        internal Troop CreateNewTroop(string troopType,int kingdomId)
        {
            var requiredTroopType = DbContext.TroopTypes.Include(t=>t.TroopLevel)
                .Where(t => t.TroopType == troopType.ToLower()).FirstOrDefault();

            // var newTroopFromRules = DbContext.TroopLevel.Where(t => t.TroopTypeId == requiredTroopType && t.Level == 1).FirstOrDefault();
            return new()
            {
                TroopTypeId = requiredTroopType.TroopTypeId,
                Level = requiredTroopType.TroopLevel.Level,
                StartedAt = 888,
                FinishedAt =999,
                KingdomId=kingdomId
            };
        }

        internal int GetTroopCost(string troopType)
        {
            return DbContext.TroopLevel.Where(t => t.TroopType.TroopType == troopType.ToLower()).FirstOrDefault().Cost; 
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
