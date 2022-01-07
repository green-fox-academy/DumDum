using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DumDum.Database;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Battles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace DumDum.Services
{
    public class BattleService
    {
        private ApplicationDbContext DbContext { get; set; }
        private AuthenticateService AuthenticateService { get; set; }
        private DumDumService DumDumService { get; set; }

        public BattleService(ApplicationDbContext dbContext, AuthenticateService authService, DumDumService dumService)
        {
            DbContext = dbContext;
            AuthenticateService = authService;
            DumDumService = dumService;
        }

        public BattleResult GetBattleResult(string authorization, int attackerKingdomId, int battleId,
            out int statusCode)
        {
            if (authorization != "")
            {
                AuthRequest request = new AuthRequest();
                request.Token = authorization.Remove(0, 7);
                var player = AuthenticateService.GetUserInfo(request);

                if (player == null)
                {
                    statusCode = 401;
                    return new BattleResult();
                }

                var battle = GetBattleById(battleId);
                var attackerTroopsLost = GetListOfTroopsLost(battle.AttackerId, battleId);
                var defenderTroopsLost = GetListOfTroopsLost(battle.DefenderId, battleId);
                var attacker = new Attacker()
                    {ResourcesStolen = new ResourcesStolen() {Food = battle.FoodStolen, Gold = battle.GoldStolen}};
                var defender = new Defender() { };
                var troopAttackerList = new List<TroopsList>();
                var troopDefenderList = new List<TroopsList>();

                foreach (var troop in attackerTroopsLost)
                {
                    var troopToAddList = new TroopsList()
                        {Quantity = troop.Quantity, Type = GetTroopTypeById(troop.Type).TroopType};
                    troopAttackerList.Add(troopToAddList);
                }

                foreach (var troop in defenderTroopsLost)
                {
                    var troopToAddList = new TroopsList()
                        {Quantity = troop.Quantity, Type = GetTroopTypeById(troop.Type).TroopType};
                    troopDefenderList.Add(troopToAddList);
                }

                attacker.TroopsLost = troopAttackerList;
                defender.TroopsLost = troopDefenderList;

                statusCode = 200;
                return new BattleResult()
                {
                    BattleId = battle.BattleId, ResolutionTime = battle.ResolutionTime,
                    BattleType = battle.BattleType,
                    Winner = DumDumService.GetPlayerById(battle.WinnerPlayerId).Username, Attacker = attacker,
                    Defender = defender
                };
            }

            statusCode = 401;
            return new BattleResult();
        }

        public BattleResponse MakeBattle(string authorization, int attackerKingdomId, BattleRequest battleRequest,
            out int statusCode)
        {
            if (authorization == "")
            {
                statusCode = 401;
                return new BattleResponse();
            }

            AuthRequest request = new AuthRequest();
            request.Token = authorization.Remove(0, 7);
            var player = AuthenticateService.GetUserInfo(request);

            if (player != null)
            {
                var kingdom = DumDumService.GetKingdomById(battleRequest.Target.KingdomId);
                var minSpeed = GetMinSpeed(attackerKingdomId);
                var attacker = DumDumService.GetPlayerByUsername(player.Ruler);
                var resolutionTime = ResolutionTimeCount(battleRequest.Target.Location.CoordinateX,
                    battleRequest.Target.Location.CoordinateY, minSpeed);
                string loser;
                var timeToStartTheBattle =
                    (int) (long) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)))
                    .TotalSeconds + resolutionTime;
                var winnerLostTroops = new List<TroopsLost>();
                var loserLostTroops = new List<TroopsLost>();
                var winner = GetWinner(attacker, kingdom, out loser, out winnerLostTroops, out loserLostTroops);
                TakeAndGiveLoot(winner, loser, out float goldStolen, out float foodStolen);
                var battle = AddBattle(battleRequest, attacker.PlayerId, resolutionTime, winner,
                    timeToStartTheBattle, (int) foodStolen, (int) goldStolen);

                foreach (var troop in winnerLostTroops)
                {
                    var troopToUpdate = DbContext.TroopsLost.FirstOrDefault(t =>
                        t.TroopLostId == troop.TroopLostId && t.PlayerId == winner);
                    if (troopToUpdate is not null)
                    {
                        troopToUpdate.BattleId = battle.BattleId;
                        DbContext.Update(troopToUpdate);
                        DbContext.SaveChanges();
                    }
                }

                foreach (var troop in loserLostTroops)
                {
                    var troopToUpdate = DbContext.TroopsLost.FirstOrDefault(t =>
                        t.TroopLostId == troop.TroopLostId &&
                        t.PlayerId == DumDumService.GetPlayerByUsername(loser).PlayerId);
                    if (troopToUpdate is not null)
                    {
                        troopToUpdate.BattleId = battle.BattleId;
                        DbContext.Update(troopToUpdate);
                        DbContext.SaveChanges();
                    }
                }

                statusCode = 200;
                return new BattleResponse()
                    {BattleId = battle.BattleId, ResolutionTime = battle.ResolutionTime};
            }

            statusCode = 401;
            return new BattleResponse();
        }

        public Battle AddBattle(BattleRequest battleRequest, int attackerId, long resolutionTime, int winnerId,
            long timeToStartTheBattle, int foodStolen, int goldStolen)
        {
            var battleToAdd = new Battle()
            {
                BattleType = battleRequest.BattleType, AttackerId = attackerId,
                DefenderId = DumDumService.GetPlayerByUsername(battleRequest.Target.Ruler).PlayerId,
                ResolutionTime = resolutionTime, WinnerPlayerId = winnerId,
                TimeToStartTheBattle = timeToStartTheBattle, FoodStolen = foodStolen, GoldStolen = goldStolen
            };

            var battle = DbContext.Battles.Add(battleToAdd).Entity;
            DbContext.SaveChanges();
            return battle;
        }

        public long ResolutionTimeCount(int coordinateX, int coordinateY, double minSpeed)
        {
            double newCoordinateX = Convert.ToDouble(coordinateX * coordinateX);
            double newCoordinateY = Convert.ToDouble(coordinateY * coordinateY);
            double toSquare = newCoordinateX + newCoordinateY;

            return Convert.ToInt64(Math.Sqrt(toSquare) * minSpeed);
        }

        public double GetMinSpeed(int kingdomId)
        {
            var kingdom = DumDumService.GetKingdomById(kingdomId);
            if (GetTroopsByKingdomId(kingdom.KingdomId) is null)
            {
                return 0;
            }

            var minSpeed = DbContext.Troops.Include(t => t.TroopType)
                .Where(t => t.KingdomId == kingdomId && t.TroopType.TroopLevel.Level == t.Level)
                .Min(t => t.TroopType.TroopLevel.Speed);

            return minSpeed;
        }

        private int GetSumOfAttackPower(Player player)
        {
            if (GetTroopsByKingdomId(player.KingdomId) is null)
            {
                return 0;
            }

            var attackPower = DbContext.Troops.Include(t => t.TroopType)
                .Where(t => t.KingdomId == player.KingdomId && t.TroopType.TroopLevel.Level == t.Level)
                .Sum(t => t.TroopType.TroopLevel.Attack);
            return (int) attackPower;
        }

        private List<Troop> GetTroopsByKingdomId(int id)
        {
            if (DbContext.Troops.Where(t => t.KingdomId == id).ToList().Count is 0)
            {
                return null;
            }

            return DbContext.Troops.Where(t => t.KingdomId == id).ToList();
        }

        private int GetSumOfDefensePower(Kingdom kingdom)
        {
            if (GetTroopsByKingdomId(kingdom.KingdomId) is null)
            {
                return 0;
            }

            var defensePower = DbContext.Troops.Include(t => t.TroopType)
                .Where(t => t.KingdomId == kingdom.KingdomId && t.TroopType.TroopLevel.Level == t.Level)
                .Sum(t => t.TroopType.TroopLevel.Defence);
            return (int) defensePower;
        }

        public int GetWinner(Player player, Kingdom kingdom, out string loser, out List<TroopsLost> winnerLostTroops,
            out List<TroopsLost> loserLostTroops)
        {
            int defenderPower = GetSumOfDefensePower(kingdom);
            int attackPower = GetSumOfAttackPower(player);
            var battleResult = attackPower - defenderPower;

            if (battleResult > 0)
            {
                loser = kingdom.Player.Username;
                TakeTroops(player.KingdomId, kingdom.KingdomId, out winnerLostTroops, out loserLostTroops);
                return player.PlayerId;
            }

            loser = player.Username;
            TakeTroops(kingdom.Player.KingdomId, player.KingdomId, out winnerLostTroops, out loserLostTroops);
            return kingdom.Player.PlayerId;
        }

        public void TakeTroops(int winnerKingdomId, int loserKingdomId, out List<TroopsLost> winnerLostTroops,
            out List<TroopsLost> loserTroopsLost)
        {
            var winner = DumDumService.GetKingdomById(winnerKingdomId);
            var loser = DumDumService.GetKingdomById(loserKingdomId);
            winnerLostTroops = new List<TroopsLost>();
            var winnersTroops = GetTroopsByKingdomId(winner.KingdomId);
            var toDivideWith = winnersTroops.Count;
            float amountDivision = toDivideWith / 100f;
            var amountWinner = amountDivision * 40;
            var winList = winnersTroops.Take((int) amountWinner);
            if (winList.Count() != 0)
            {
                foreach (var troop in winList)
                {
                    var lost = new TroopsLost()
                    {
                        Quantity = GetTroopsByKingdomId(winner.KingdomId)
                            .Count(t => t.TroopTypeId.Equals(troop.TroopTypeId)),
                        Type = troop.TroopTypeId, PlayerId = winner.PlayerId
                    };
                    var lostTroop = DbContext.TroopsLost.Add(lost).Entity;
                    winnerLostTroops.Add(lostTroop);
                    DbContext.Troops.Remove(troop);
                    DbContext.SaveChanges();
                }

                winner.Troops.RemoveRange(1, (int) amountWinner);
            }
            else
            {
                var lost = new TroopsLost()
                {
                    Quantity = 0,
                    Type = 0, PlayerId = winner.PlayerId
                };
                winnerLostTroops.Add(lost);
            }

            loserTroopsLost = new List<TroopsLost>();
            if (GetTroopsByKingdomId(loserKingdomId) is not null)
            {
                foreach (var troop in GetTroopsByKingdomId(loserKingdomId))
                {
                    var troopToAdd = new TroopsLost()
                    {
                        Quantity = loser.Troops.Count(t => t.TroopTypeId.Equals(troop.TroopTypeId)),
                        Type = troop.TroopTypeId, PlayerId = loser.PlayerId
                    };

                    if (!loserTroopsLost.Any(t => t.Type.Equals(troopToAdd.Type)))
                    {
                        var lostTroop = DbContext.TroopsLost.Add(troopToAdd).Entity;
                        loserTroopsLost.Add(lostTroop);
                    }

                    DbContext.Troops.Remove(troop);
                    DbContext.SaveChanges();
                }
            }
        }

        public void TakeAndGiveLoot(int winnerId, string loser, out float goldStolen, out float foodStolen)
        {
            var kingdomOfWinner = DumDumService.GetPlayerById(winnerId).Kingdom;
            var kingdomOfLoser = DumDumService.GetPlayerByUsername(loser).Kingdom;
            var amountOfGold = DumDumService.GetGoldAmountOfKingdom(kingdomOfLoser.KingdomId);
            float amountOfGoldToTakeOrGive = amountOfGold / 100f;
            goldStolen = amountOfGoldToTakeOrGive * 20;
            DumDumService.TakeGold(kingdomOfLoser.KingdomId, (int) goldStolen);
            float amountOfFood = DumDumService.GetFoodAmountOfKingdom(kingdomOfLoser.KingdomId);
            var amountOfFoodToTakeOrGive = amountOfFood / 100;
            foodStolen = amountOfFoodToTakeOrGive * 20;
            DumDumService.TakeFood(kingdomOfLoser.KingdomId, (int) foodStolen);
            DumDumService.GiveGold(kingdomOfWinner.KingdomId, (int) goldStolen);
            DumDumService.GiveFood(kingdomOfWinner.KingdomId, (int) foodStolen);
        }

        public Battle GetBattleById(int id)
        {
            return DbContext.Battles.FirstOrDefault(b => b.BattleId == id);
        }

        private TroopTypes GetTroopTypeById(int troopTypeId)
        {
            return DbContext.TroopTypes.FirstOrDefault(t => t.TroopTypeId == troopTypeId);
        }

        private List<TroopsLost> GetListOfTroopsLost(int playerId, int battleId)
        {
            return DbContext.TroopsLost.Where(t => t.PlayerId == playerId && t.BattleId == battleId).ToList();
        }
    }
}