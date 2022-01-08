using DumDum.Interfaces;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Kingdom;
using DumDum.Models.JsonEntities.Troops;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DumDum.Services
{
    public class TroopService
    {
        private IAuthenticateService AuthenticateService { get; set; }
        private IDumDumService DumDumService { get; set; }
        private IUnitOfWork UnitOfWork { get; set; }
        public TroopService(IAuthenticateService authService, IDumDumService dumService, IUnitOfWork unitOfWork)
        {
            AuthenticateService = authService;
            DumDumService = dumService;
            UnitOfWork = unitOfWork;
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
            return UnitOfWork.Troops.GetTroops(kingdomId);
        }

        internal string UpgradeTroops(string authorization, TroopUpgradeRequest troopUpdateReq, int kingdomId, out int statusCode)
        {
            var player = AuthenticateService.GetUserInfo(new AuthRequest() { Token = authorization });
            var possibleTroopTypes = UnitOfWork.TroopTypes.PossibleTroopTypesToUpgrade();
            var goldAmount = DumDumService.GetGoldAmountOfKingdom(kingdomId);

            if (troopUpdateReq == null || string.IsNullOrEmpty(troopUpdateReq.Type) || !possibleTroopTypes.Contains(troopUpdateReq.Type.ToLower()))
            {
                statusCode = 404;
                return "Request was not done correctly!";
            }
            if (player != null && player.KingdomId == kingdomId && troopUpdateReq != null)
            {
                var troopUpgradeCost = GetTroopUpdateCost(troopUpdateReq.Type.ToLower());
                var amountOfTroopsToUpdate = CountTroopsByType(troopUpdateReq.Type.ToLower(), kingdomId);
                var troopTypeIdToBeUpgraded = GetTroupTypeIdByTroupTypeName(troopUpdateReq.Type.ToLower());
                var currentLevelOfTownhall = CurrentLevelOfTownhall(kingdomId);
                var currentLevelOfTroops = CurrentLevelOTroops(kingdomId, troopUpdateReq.Type.ToLower());
                var maximumLevelPossible = UnitOfWork.TroopLevels.MaximumLevelPossible();
                var timeRequiredToUpgradeTroop = UnitOfWork.TroopLevels.Find(t => t.Level == currentLevelOfTroops + 1 && t.TroopTypeId == troopTypeIdToBeUpgraded)
                   .Select(t => t.ConstTime).FirstOrDefault();

                if (IsUpgradeInProgress(kingdomId, troopUpdateReq.Type))
                {
                    statusCode = 407;
                    return "Creation or upgrade of this type of troop is already in progress";
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
                UnitOfWork.Troops.UpgradeTroops(troopTypeIdToBeUpgraded, kingdomId, timeRequiredToUpgradeTroop);
                UnitOfWork.Complete();
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
            var possibleTroopTypes = UnitOfWork.TroopTypes.PossibleTroopTypes();

            if (troopCreationReq == null || String.IsNullOrEmpty(troopCreationReq.Type) || troopCreationReq.Quantity == 0 || 
                !possibleTroopTypes.Contains(troopCreationReq.Type.ToLower()))
            {
                statusCode = 404;
                return new List<TroopsResponse>();
            }
            if (player != null && player.KingdomId == kingdomId)
            {
                var currentLevelOfTroops = CurrentLevelOTroops(kingdomId, troopCreationReq.Type.ToLower());
                int newTroopCost = GetTroopCreationCost(troopCreationReq.Type.ToLower(), currentLevelOfTroops);

                if (goldAmount < newTroopCost * troopCreationReq.Quantity)
                {
                    statusCode = 400;
                    return new List<TroopsResponse>();
                }
                for (int i = 0; i < troopCreationReq.Quantity; i++)
                {
                    var newTroop = CreateNewTroop(troopCreationReq.Type.ToLower(), kingdomId);
                    UnitOfWork.Troops.Add(newTroop);
                    UnitOfWork.Complete();
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
            var requiredTroopTypeId = UnitOfWork.TroopTypes.Find(t => t.TroopType.ToLower() == troopType.ToLower()).FirstOrDefault().TroopTypeId;
            var requiredTroop = UnitOfWork.TroopLevels.Find(t => t.TroopTypeId == requiredTroopTypeId && t.Level == 1).FirstOrDefault();
            var requiredTroopTypeAlreadyInKingdom = UnitOfWork.Troops.Find(t => t.TroopTypeId == requiredTroopTypeId && t.KingdomId == kingdomId).FirstOrDefault();
            var timeRequiredToCreateTroop = UnitOfWork.TroopLevels.Find(t => t.TroopTypeId == requiredTroopTypeId && t.Level == 1).Select(t => t.ConstTime).FirstOrDefault();

            if (requiredTroopTypeAlreadyInKingdom != null)
            {
                return new()
                {
                    TroopTypeId = requiredTroopTypeId,
                    Level = requiredTroopTypeAlreadyInKingdom.Level,
                    StartedAt = (int)DateTimeOffset.Now.ToUnixTimeSeconds(),
                    FinishedAt = (int)DateTimeOffset.Now.ToUnixTimeSeconds() + timeRequiredToCreateTroop,
                    KingdomId = kingdomId
                };
            }
            return new()
            {
                TroopTypeId = requiredTroopTypeId,
                Level = requiredTroop.Level,
                StartedAt = (int)DateTimeOffset.Now.ToUnixTimeSeconds(),
                FinishedAt = (int)DateTimeOffset.Now.ToUnixTimeSeconds() + timeRequiredToCreateTroop,
                KingdomId = kingdomId
            };
        }

        internal bool DoesAcademyExist(int kingdomId)
        {
            return UnitOfWork.Buildings.Find(b => b.BuildingType.ToLower() == "academy" && b.KingdomId == kingdomId).Any();
        }

        internal bool IsUpgradeInProgress(int kingdomId, string troopType)
        {
            return (int)DateTimeOffset.Now.ToUnixTimeSeconds() < UnitOfWork.Troops.FinishedAtTimeTroop(troopType, kingdomId);
        }

        internal int GetTroopCreationCost(string troopType, int troopCreationLevel)
        {
            if (troopCreationLevel > 0)
            {
                return UnitOfWork.TroopLevels.TroopCreationHigherLevel(troopType, troopCreationLevel).Cost;
            }
            return 0;
        }

        internal int GetTroopUpdateCost(string troopType)
        {
            var currentLevelOfTroops = GetTroupTypeIdByTroupTypeName(troopType.ToLower());
            var troopToUpdate = UnitOfWork.TroopLevels.Find(t => t.TroopTypeId == GetTroupTypeIdByTroupTypeName(troopType.ToLower()) && t.Level == currentLevelOfTroops + 1)
                    .FirstOrDefault();
            if (troopToUpdate != null)
            {
                return troopToUpdate.Cost;
            }
            return 0;
        }

        internal int CountTroopsByType(string troopType, int kingdomId)
        {
            var troopsCountByType = UnitOfWork.Troops.Find(t => t.TroopType.TroopType == troopType.ToLower() && t.KingdomId == kingdomId);
            if (troopsCountByType != null && troopType != null)
            {
                return troopsCountByType.Count();
            }
            return 0;
        }

        internal int GetTroupTypeIdByTroupTypeName(string troopType)
        {
            var TroupTypeIdByTroupTypeName = UnitOfWork.TroopTypes.Find(t => t.TroopType == troopType.ToLower()).FirstOrDefault();
            if (TroupTypeIdByTroupTypeName != null)
            {
                return TroupTypeIdByTroupTypeName.TroopTypeId;
            }
            return 0;
        }

        internal int CurrentLevelOfTownhall(int kingdomId)
        {
            var townhallLevel = UnitOfWork.Buildings.Find(b => b.KingdomId == kingdomId && b.BuildingType.ToLower() == "townhall").FirstOrDefault();
            if (townhallLevel != null)
            {
                return townhallLevel.Level;
            }
            return 0;
        }

        internal int CurrentLevelOTroops(int kingdomId, string troopType)
        {
            var currentTrooptype = GetTroupTypeIdByTroupTypeName(troopType);
            var currentLevelOTroops = UnitOfWork.Troops.Find(t => t.KingdomId == kingdomId && t.TroopTypeId == currentTrooptype).FirstOrDefault();
            if (currentLevelOTroops != null)
            {
                return currentLevelOTroops.Level;
            }
            return 0;
        }

        public TroopsLeaderboardResponse GetTroopsLeaderboard()
        {
            TroopsLeaderboardResponse response = new();
            List<TroopsPoint> pointsList = new();
            var kingdoms = UnitOfWork.Kingdoms.GetAllKingdomsIncludePlayer();
            foreach (var kingdom in kingdoms)
            {
                var troopPoint = new TroopsPoint()
                {
                    Ruler = kingdom.Player.Username,
                    Kingdom = kingdom.KingdomName,
                    Troops = UnitOfWork.Troops.Find(t => t.KingdomId == kingdom.KingdomId).Count(),
                    Points = GetAllTroopsConsumptionInKingdom(kingdom.KingdomId)
                };
                pointsList.Add(troopPoint);
            }

            response.Result = pointsList.OrderByDescending(x => x.Points).ToList();

            return response;
        }

        public double GetAllTroopsConsumptionInKingdom(int kingdomId)
        {
            var consumptionOfAllTroopsInKingdom = 0.0;

                var troopsInKingdom = UnitOfWork.Troops.Find(t => t.KingdomId == kingdomId).ToList();
                foreach (var troop in troopsInKingdom)
                {
                    consumptionOfAllTroopsInKingdom += UnitOfWork.TroopLevels.Find(t => t.TroopTypeId == troop.TroopTypeId && t.Level == troop.Level)
                        .Select(t => t.Consumption)
                        .FirstOrDefault();
                }

            return consumptionOfAllTroopsInKingdom;
        }

        public KingdomsLeaderboardResponse GetKingdomsLeaderboard()
        {
            KingdomsLeaderboardResponse response = new();

            List<KingdomPoints> pointsList = new();
            var kingdoms = UnitOfWork.Kingdoms.GetAllKingdomsIncludePlayer();

            foreach (var kingdom in kingdoms)
            {
                var kingdomPoint = new KingdomPoints()
                {
                    Ruler = kingdom.Player.Username,
                    Kingdom = kingdom.KingdomName,
                    Points = GetAllTroopsConsumptionInKingdom(kingdom.KingdomId)
                            + UnitOfWork.Buildings.GetAllBuildingsConsumptionInKingdom(kingdom)
                };
                pointsList.Add(kingdomPoint);
            }

            response.Response = pointsList.OrderByDescending(x => x.Points).ToList();

            return response;
        }

        public List<Troop> GetActiveTroops()
        {
            return UnitOfWork.Troops.Find(t => t.FinishedAt < (int)DateTimeOffset.Now.ToUnixTimeSeconds()).ToList();
        }
        public bool IsTroopActive(Troop troop)
        {
            return troop.FinishedAt < (int)DateTimeOffset.Now.ToUnixTimeSeconds();
        }
    }
}
