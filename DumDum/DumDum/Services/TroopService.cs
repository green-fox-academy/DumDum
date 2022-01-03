﻿using DumDum.Database;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Troops;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Kingdom;
using DumDum.Models.JsonEntities.Troops;
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
            List<TroopsResponse> troops = DbContext.Troops.Where(t => t.KingdomId == kingdomId).Include(t => t.TroopType.TroopLevel).ToList().
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

        internal string UpgradeTroops(string authorization, TroopUpgradeRequest troopUpdateReq, int kingdomId, out int statusCode)
        {
            var player = AuthenticateService.GetUserInfo(new AuthRequest() { Token = authorization });
            var goldAmount = DumDumService.GetGoldAmountOfKingdom(kingdomId);
            var possibleTroopTypes = DbContext.TroopTypes.Where(t => t.TroopType.ToLower() != "senator").Select(t => t.TroopType.ToLower()).ToList();
            var amountOfTroopsToUpdate = CountTroopsByType(troopUpdateReq.Type.ToLower(), kingdomId);
            var troopUpgradeCost = GetTroopUpdateCost(troopUpdateReq.Type.ToLower());
            var troopIdToBeUpgraded = GetTroupTypeIdByTroupTypeName(troopUpdateReq.Type.ToLower());
            var currentLevelOfTownhall = CurrentLevelOfTownhall(kingdomId);
            var currentLevelOfTroops = CurrentLevelOTroops(kingdomId, troopUpdateReq.Type.ToLower());
            var maximumLevelPossible = DbContext.TroopLevel.Select(t => t.Level).Max();

            if (player != null && player.KingdomId == kingdomId)
            {
                if (string.IsNullOrEmpty(troopUpdateReq.Type) || !possibleTroopTypes.Contains(troopUpdateReq.Type.ToLower()))
                {
                    statusCode = 404;
                    return "Request was not done correctly!";
                }
                if (!DoesAcademyExist(kingdomId))
                {
                    statusCode = 406;
                    return "Build Academy first";
                }

                if (maximumLevelPossible <= currentLevelOfTroops)
                {
                    statusCode = 405;
                    return "Maximum level reached";
                }
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
                if (currentLevelOfTownhall <= currentLevelOfTroops)
                {
                    statusCode = 403;
                    return "Upgrade townhall first";
                }
                DbContext.Troops.Where(t => t.TroopTypeId == troopIdToBeUpgraded && t.KingdomId == kingdomId).ToList().ForEach(t => t.Level = t.Level + 1);
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
            var currentLevelOfTroops = CurrentLevelOTroops(kingdomId, troopCreationReq.Type.ToLower());

            if (troopCreationReq.Type == null || troopCreationReq.Quantity == 0 || !possibleTroopTypes.Contains(troopCreationReq.Type.ToLower()))
            {
                statusCode = 404;
                return new List<TroopsResponse>();
            }
            if (player != null && player.KingdomId == kingdomId)
            {
                int newTroopCost = GetTroopCreationCost(troopCreationReq.Type.ToLower(), currentLevelOfTroops);

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
            var requiredTroopTypeId = DbContext.TroopTypes.Where(t => t.TroopType.ToLower() == troopType.ToLower()).FirstOrDefault().TroopTypeId;
            var requiredTroop = DbContext.TroopLevel.Where(t => t.TroopTypeId == requiredTroopTypeId && t.Level == 1).FirstOrDefault();
            var requiredTroopTypeAlreadyInKingdom = DbContext.Troops.Where(t => t.TroopTypeId == requiredTroopTypeId && t.KingdomId == kingdomId).FirstOrDefault();

            if (requiredTroopTypeAlreadyInKingdom != null)
            {
                return new()
                {
                    TroopTypeId = requiredTroopTypeId,
                    Level = requiredTroopTypeAlreadyInKingdom.Level,
                    StartedAt = 888,
                    FinishedAt = 999,
                    KingdomId = kingdomId
                };
            }
            return new()
            {
                TroopTypeId = requiredTroopTypeId,
                Level = requiredTroop.Level,
                StartedAt = 888,
                FinishedAt = 999,
                KingdomId = kingdomId
            };
        }

        internal bool DoesAcademyExist(int kingdomId)
        {
            return DbContext.Buildings.Where(b => b.BuildingType.ToLower() == "academy" && b.KingdomId == kingdomId).Any();
        }

        internal int GetTroopCreationCost(string troopType, int troopCreationLevel)
        {
            if (troopCreationLevel > 0)
            {
                var troopToCreateHigherLevel = DbContext.TroopLevel.Where(t => t.TroopType.TroopType == troopType.ToLower() && t.Level == troopCreationLevel).FirstOrDefault();
                return troopToCreateHigherLevel.Cost;
            }
            var troopToCreateLevelOne = DbContext.TroopLevel.Where(t => t.TroopType.TroopType == troopType.ToLower() && t.Level == troopCreationLevel).FirstOrDefault();
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

        internal int CountTroopsByType(string troopType, int kingdomId)
        {
            var troopsCountByType = DbContext.Troops.Where(t => t.TroopType.TroopType == troopType.ToLower() && t.KingdomId == kingdomId);
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

        internal int CurrentLevelOfTownhall(int kingdomId)
        {
            var townhallLevel = DbContext.Buildings.Where(b => b.KingdomId == kingdomId && b.BuildingType.ToLower() == "townhall").FirstOrDefault();
            if (townhallLevel != null)
            {
                return townhallLevel.Level;
            }
            return 0;
        }

        internal int CurrentLevelOTroops(int kingdomId, string troopType)
        {
            var currentTrooptype = GetTroupTypeIdByTroupTypeName(troopType);
            var currentLevelOTroops = DbContext.Troops.Where(t => t.KingdomId == kingdomId && t.TroopTypeId == currentTrooptype).FirstOrDefault();
            if (currentLevelOTroops != null)
            {
                return currentLevelOTroops.Level;
            }
            return 0;
        }
    }
}
