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
using System.Threading.Tasks;

namespace DumDum.Services
{
    public class TroopService
    {
        private AuthenticateService AuthenticateService { get; set; }
        private DumDumService DumDumService { get; set; }
        private IUnitOfWork UnitOfWork { get; set; }
        public TroopService(AuthenticateService authService, DumDumService dumService, IUnitOfWork unitOfWork)
        {
            AuthenticateService = authService;
            DumDumService = dumService;
            UnitOfWork = unitOfWork;
        }

        internal async Task<(GetTroopsResponse, int)> ListTroops(string authorization, int kingdomId)
        {
            
            var player = await AuthenticateService.GetUserInfo(new AuthRequest() { Token = authorization });

            if (player != null && player.KingdomId == kingdomId)
            {
                var kingdom = DumDumService.GetKingdomById(player.KingdomId);
                var response = new GetTroopsResponse(new KingdomResponse(kingdom), await GetTroops(player.KingdomId) );
                return (response, 200);
            }
            return (null, 401);
        }

        internal async Task<List<TroopsResponse>> GetTroops(int kingdomId)
        {
            return await UnitOfWork.Troops.GetTroops(kingdomId);
        }

        internal async Task<(string, int)> UpgradeTroops(string authorization, TroopUpgradeRequest troopUpdateReq, int kingdomId)
        {
            var player = await AuthenticateService.GetUserInfo(new AuthRequest() { Token = authorization });
            var possibleTroopTypes = UnitOfWork.TroopTypes.PossibleTroopTypesToUpgrade();

            if (troopUpdateReq == null || string.IsNullOrEmpty(troopUpdateReq.Type) || !possibleTroopTypes.Result.Contains(troopUpdateReq.Type.ToLower()))
            {
                return ("Request was not done correctly!", 404);
            }
            if (player != null && player.KingdomId == kingdomId && troopUpdateReq != null)
            {
                var goldAmount = DumDumService.GetGoldAmountOfKingdom(kingdomId);
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
                    return ("Creation or upgrade of this type of troop is already in progress", 407);
                }
                if (!DoesAcademyExist(kingdomId))
                {
                    return ("Build Academy first", 406);
                }
                if (maximumLevelPossible.Result <= currentLevelOfTroops)

                {
                    return ("Maximum level reached", 405);
                }
                if (amountOfTroopsToUpdate == 0)
                {
                    return ("You don't have this type of troops!", 402);
                }

                if (goldAmount < troopUpgradeCost * amountOfTroopsToUpdate)
                {
                    return ("You don't have enough gold to upgrade this type of troops!", 400);
                }
                if (currentLevelOfTownhall <= currentLevelOfTroops)
                {
                    return ("Upgrade townhall first", 403);
                }
                UnitOfWork.Troops.UpgradeTroops(troopTypeIdToBeUpgraded, kingdomId, timeRequiredToUpgradeTroop);
                UnitOfWork.Complete();
                DumDumService.TakeGold(kingdomId, troopUpgradeCost * amountOfTroopsToUpdate);
                return ("Ok", 200);
            }
            return ("This kingdom does not belong to authenticated player", 401);
        }

        internal async Task<(List<TroopsResponse>, int)> CreateTroops(string authorization, TroopCreationRequest troopCreationReq, int kingdomId)
        {
            var player = await AuthenticateService.GetUserInfo(new AuthRequest() { Token = authorization });
            if (player != null && player.KingdomId == kingdomId)
            {
                var goldAmount = DumDumService.GetGoldAmountOfKingdom(kingdomId);
                var createdTroops = new List<TroopsResponse>();
                var possibleTroopTypes = UnitOfWork.TroopTypes.PossibleTroopTypes();

                if (troopCreationReq == null || String.IsNullOrEmpty(troopCreationReq.Type) || troopCreationReq.Quantity == 0 || 
                    !possibleTroopTypes.Result.Contains(troopCreationReq.Type.ToLower()))
                {
                    return (new List<TroopsResponse>(), 404);
                }
                var currentLevelOfTroops = CurrentLevelOTroops(kingdomId, troopCreationReq.Type.ToLower());
                int newTroopCost = GetTroopCreationCost(troopCreationReq.Type.ToLower(), currentLevelOfTroops);

                if (goldAmount < newTroopCost * troopCreationReq.Quantity)
                {
                    return (new List<TroopsResponse>(), 400);
                }
                for (int i = 0; i < troopCreationReq.Quantity; i++)
                {
                    var newTroop = CreateNewTroop(troopCreationReq.Type.ToLower(), kingdomId);
                    UnitOfWork.Troops.Add(newTroop);
                    UnitOfWork.Complete();
                    DumDumService.TakeGold(kingdomId, newTroopCost);
                    createdTroops.Add(new TroopsResponse(newTroop));
                }
                return (createdTroops, 200);
            }
            return (new List<TroopsResponse>(), 401);
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
            return (int)DateTimeOffset.Now.ToUnixTimeSeconds() < UnitOfWork.Troops.FinishedAtTimeTroop(troopType, kingdomId).Result;
        }

        internal int GetTroopCreationCost(string troopType, int troopCreationLevel)
        {
            if (troopCreationLevel > 0)
            {
                return UnitOfWork.TroopLevels.TroopCreationHigherLevel(troopType, troopCreationLevel).Result.Cost;
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

        public async Task<TroopsLeaderboardResponse> GetTroopsLeaderboard()
        {
            TroopsLeaderboardResponse response = new();
            List<TroopsPoint> pointsList = new();
            var kingdoms = await UnitOfWork.Kingdoms.GetAllKingdomsIncludePlayer();
            foreach (var kingdom in kingdoms)
            {
                var troopPoint = new TroopsPoint(kingdom, GetAllTroopsConsumptionInKingdom(kingdom.KingdomId).Result,
                    UnitOfWork.Troops.Find(t => t.KingdomId == kingdom.KingdomId).Count());

                pointsList.Add(troopPoint);
            }

            response.Result = pointsList.OrderByDescending(x => x.Points).ToList();

            return response;
        }

        public async Task<double> GetAllTroopsConsumptionInKingdom(int kingdomId)
        {
            var consumptionOfAllTroopsInKingdom = 0.0;

                var troopsInKingdom = UnitOfWork.Troops.Find(t => t.KingdomId == kingdomId).ToList();
                foreach (var troop in troopsInKingdom)
                {
                    consumptionOfAllTroopsInKingdom += UnitOfWork.TroopLevels.Find(t => t.TroopTypeId == troop.TroopTypeId && t.Level == troop.Level)
                        .Select(t => t.Consumption)
                        .FirstOrDefault();
                }
                return await Task.FromResult(consumptionOfAllTroopsInKingdom);
        }

        public async Task<KingdomsLeaderboardResponse> GetKingdomsLeaderboard()
        {
            KingdomsLeaderboardResponse response = new();

            List<KingdomPoints> pointsList = new();
            var kingdoms = await UnitOfWork.Kingdoms.GetAllKingdomsIncludePlayer();

            foreach (var kingdom in kingdoms)
            {
                var kingdomPoint = new KingdomPoints(kingdom, (GetAllTroopsConsumptionInKingdom(kingdom.KingdomId).Result) + ( UnitOfWork.Buildings.GetAllBuildingsConsumptionInKingdom(kingdom).Result));
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
