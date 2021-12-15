using DumDum.Database;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
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
                int newTroopCost = CreateNewTroop(troopCreationReq.Type.ToLower()).Cost;

                if (goldAmount < newTroopCost * troopCreationReq.Quantity)
                {
                    statusCode = 400;
                    return new List<TroopsResponse>();
                }
                for (int i = 0; i < troopCreationReq.Quantity; i++)
                {
                    var newTroop = CreateNewTroop(troopCreationReq.Type.ToLower());
                    newTroop.KingdomId = kingdomId;
                    DbContext.Troops.Add(newTroop);
                    DbContext.SaveChanges();
                    DumDumService.TakeGold(kingdomId, newTroop.Cost);
                    createdTroops.Add(new TroopsResponse()
                    {
                        TroopId = newTroop.TroopId,
                        TroopType = newTroop.TroopType,
                        Level = newTroop.Level,
                        HP = newTroop.HP,
                        Attack = newTroop.Attack,
                        Defence = newTroop.Defence,
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

        internal Troop CreateNewTroop(string troopType)
        {
            var requiredTroopType = DbContext.TroopTypes.Where(t => t.TroopType == troopType.ToLower()).Select(t => t.TroopTypeId).FirstOrDefault();
            var newTroopFromRules = DbContext.TroopLevel.Where(t => t.TroopTypeId == requiredTroopType && t.Level == 1).FirstOrDefault();
            return new()
            {
                TroopType = troopType.ToLower(),
                Level = newTroopFromRules.Level,
                HP = newTroopFromRules.HP,
                Attack = newTroopFromRules.Attack,
                Defence = newTroopFromRules.Defence,
                CarryCap = newTroopFromRules.CarryCap,
                Consumption = newTroopFromRules.Consumption,
                Speed = newTroopFromRules.Speed,
                Cost = newTroopFromRules.Cost
            };
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
