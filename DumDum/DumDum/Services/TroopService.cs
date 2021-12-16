using DumDum.Database;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Troops;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;

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
            var player = AuthenticateService.GetUserInfo(new AuthRequest() { Token = authorization });

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
            List<TroopsResponse> troops = DbContext.Troops.Where(t => t.KingdomId == kingdomId).Include(t => t.TroopType).ToList().
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

        internal string UpgradeTroops(string authorization, TroopUpdateRequest troopUpdateReq, int kingdomId, out int statusCode)
        {
            var player = AuthenticateService.GetUserInfo(new AuthRequest() { Token = authorization });
            var goldAmount = DumDumService.GetGoldAmountOfKingdom(kingdomId);
            var possibleTroopTypes = DbContext.TroopTypes.Select(t => t.TroopType.ToLower()).ToList();
            var amountOfTroopsToUpdate = CountTroopsByType(troopUpdateReq.Type.ToLower());
            var troopUpgradeCost = GetTroopUpdateCost(troopUpdateReq.Type.ToLower());
            var troopIdToBeUpgraded = GetTroupTypeIdByTroupTypeName(troopUpdateReq.Type.ToLower());

            if (string.IsNullOrEmpty(troopUpdateReq.Type) || !possibleTroopTypes.Contains(troopUpdateReq.Type.ToLower()))
            {
                statusCode = 404;
                return "Request was not done correctly!";
            }
            if (player != null && player.KingdomId == kingdomId)
            {
                if (amountOfTroopsToUpdate == 0)
                {
                    statusCode = 402;
                    return "You don't have this type of troops!";
                }
                if (goldAmount < troopUpgradeCost * amountOfTroopsToUpdate)
                {
                    statusCode = 400;
                    return "You don't have enough gold to upgrade this type of troops!";
                }

                DbContext.Troops.Where(t => t.TroopTypeId == troopIdToBeUpgraded).ToList().ForEach(t => t.Level = t.Level + 1);
                DbContext.SaveChanges();
                DumDumService.TakeGold(kingdomId, troopUpgradeCost * amountOfTroopsToUpdate);
                statusCode = 200;
                return "Ok";
            }
            statusCode = 401;
            return "This kingdom does not belong to authenticated player";
        }

        internal List<TroopsResponse> CreateTroops(string authorization, TroopCreationRequest troopCreationReq, int kingdomId, out int statusCode)
        {
            var player = AuthenticateService.GetUserInfo(new AuthRequest() { Token = authorization });
            var goldAmount = DumDumService.GetGoldAmountOfKingdom(kingdomId);
            var createdTroops = new List<TroopsResponse>();
            var possibleTroopTypes = DbContext.TroopTypes.Select(t => t.TroopType.ToLower()).ToList();

            if (troopCreationReq.Type == null || troopCreationReq.Quantity == 0 || !possibleTroopTypes.Contains(troopCreationReq.Type.ToLower()))
            {
                statusCode = 404;
                return new List<TroopsResponse>();
            }
            if (player != null && player.KingdomId == kingdomId)
            {
                int newTroopCost = GetTroopCreationCost(troopCreationReq.Type.ToLower());

                if (goldAmount < newTroopCost * troopCreationReq.Quantity)
                {
                    statusCode = 400;
                    return new List<TroopsResponse>();
                }
                for (int i = 0; i < troopCreationReq.Quantity; i++)
                {
                    var newTroop = CreateNewTroop(troopCreationReq.Type.ToLower(), kingdomId);
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

        internal Troop CreateNewTroop(string troopType, int kingdomId)
        {
            var requiredTroopType = DbContext.TroopTypes.Include(t => t.TroopLevel)
                .Where(t => t.TroopType == troopType.ToLower()).FirstOrDefault();

            return new()
            {
                TroopTypeId = requiredTroopType.TroopTypeId,
                Level = requiredTroopType.TroopLevel.Level,
                StartedAt = 888,
                FinishedAt = 999,
                KingdomId = kingdomId
            };
        }

        internal int GetTroopCreationCost(string troopType)
        {
            var troopToCreate = DbContext.TroopLevel.Where(t => t.TroopType.TroopType == troopType.ToLower()).FirstOrDefault();
            if (troopToCreate != null)
            {
                return troopToCreate.Cost;
            }
            return 0;
        }

        internal int GetTroopUpdateCost(string troopType)
        {
            var currentLevelOfTroops = GetTroupTypeIdByTroupTypeName(troopType.ToLower());
            var troopToUpdate = DbContext.TroopLevel.Where(t => t.TroopTypeId == GetTroupTypeIdByTroupTypeName(troopType.ToLower()) && t.Level == currentLevelOfTroops + 1)
                    .FirstOrDefault();
            if (troopToUpdate != null)
            {
                return troopToUpdate.Cost;
            }
            return 0;
        }

        internal int CountTroopsByType(string troopType)
        {
            var troopsCountByType = DbContext.Troops.Where(t => t.TroopType.TroopType == troopType.ToLower());
            if (troopsCountByType != null)
            {
                return troopsCountByType.Count();
            }
            return 0;
        }

        internal int GetTroupTypeIdByTroupTypeName(string troopType)
        {
            var TroupTypeIdByTroupTypeName = DbContext.TroopTypes.Where(t => t.TroopType == troopType.ToLower()).FirstOrDefault();
            if (TroupTypeIdByTroupTypeName != null)
            {
                return TroupTypeIdByTroupTypeName.TroopTypeId;
            }
            return 0;
        }

        internal int GetTownhallLevel()
        {
            var townhallLevel = DbContext.TroopTypes.Where(t => t.TroopType == troopType.ToLower()).FirstOrDefault();
            if (TroupTypeIdByTroupTypeName != null)
            {
                return TroupTypeIdByTroupTypeName.TroopTypeId;
            }
            return 0;
        }
    }
}
