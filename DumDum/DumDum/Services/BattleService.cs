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

        public BattleResult GetBattleResult(string authorization, int defenderKingdomId, int battleId,
            out int statusCode)
        {
            if (authorization != "")
            {
                AuthRequest request = new AuthRequest();
                request.Token = authorization.Remove(0, 7);
                var player = AuthenticateService.GetUserInfo(request);

                if (player != null)
                {
                    var battle = GetBattleById(battleId);
                    var attacker = GetAttackerByBattleId(battleId);
                    var defender = GetDefenderByBattleId(battleId);
                    statusCode = 200;
                    return new BattleResult()
                    {
                        BattleId = battle.BattleId, ResolutionTime = battle.ResolutionTime,
                        BattleType = battle.BattleType, Winner = battle.Winner, Attacker = attacker, Defender = defender
                    };
                }

                statusCode = 401;
                return new BattleResult();
            }

            statusCode = 401;
            return new BattleResult();
        }

        public Attacker GetAttackerByBattleId(int battleId)
        {
            return DbContext.Attackers.FirstOrDefault(a => a.BattleId == battleId);
        }

        public Defender GetDefenderByBattleId(int battleId)
        {
            return DbContext.Defenders.FirstOrDefault(a => a.BattleId == battleId);
        }

        public BattleResponse MakeBattle(string authorization, int defenderKingdomId, BattleRequest battleRequest,
            out int statusCode)
        {
            if (authorization != "")
            {
                AuthRequest request = new AuthRequest();
                request.Token = authorization.Remove(0, 7);
                var player = AuthenticateService.GetUserInfo(request);

                if (player != null)
                {
                    var kingdom = DumDumService.GetKingdomById(defenderKingdomId);
                    var minSpeed = GetMinSpeed(player.KingdomId);
                    var attacker = DumDumService.GetPlayerByUsername(player.Ruler);
                    var resolutionTime = ResolutionTimeCount(battleRequest.Target.Location.CoordinateX,
                        battleRequest.Target.Location.CoordinateY, minSpeed);
                    string loser;
                    var timeToStartTheBattle =
                        (int) (long) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds + resolutionTime;
                    var winnerLostTroops = new List<TroopsList>();
                    var loserLostTroops = new List<TroopsList>();
                    var winner = GetWinner(attacker, kingdom, out loser, out winnerLostTroops, out loserLostTroops);
                    float goldStolen;
                    float foodStolen;
                    TakeAndGiveLoot(winner, loser, out goldStolen, out foodStolen);
                    var battle = AddBattle(battleRequest, player.Ruler, resolutionTime, winner, timeToStartTheBattle);
                    if (player.Ruler == winner)
                    {
                        var resourcesStolen = new ResourcesStolen() {Food = (int) foodStolen, Gold = (int) goldStolen};
                        var addedAttacker = AddAttacker(player.Ruler, battle.BattleId, winnerLostTroops, resourcesStolen);
                        var addedDefender = AddDefender(battleRequest.Target.Ruler, battle.BattleId, loserLostTroops);
                        battle.Attacker = addedAttacker;
                        battle.Defender = addedDefender;
                        DbContext.Update(battle);
                    }
                    else
                    {
                        var loserResourcesStolen = new ResourcesStolen() {Food = 0, Gold = 0};
                        var addedAttacker = AddAttacker(player.Ruler, battle.BattleId, loserLostTroops, loserResourcesStolen);
                        var addedDefender = AddDefender(battleRequest.Target.Ruler, battle.BattleId, winnerLostTroops);
                        battle.Attacker = addedAttacker;
                        battle.Defender = addedDefender;
                        DbContext.Update(battle);
                    }

                    statusCode = 200;
                    return new BattleResponse()
                        {BattleId = battle.BattleId, ResolutionTime = battle.ResolutionTime};
                }

                statusCode = 401;
                return new BattleResponse();
            }

            statusCode = 401;
            return new BattleResponse();
        }

        public Battle AddBattle(BattleRequest battleRequest, string ruler, long resolutionTime, string winner,
            long timeToStartTheBattle)
        {
            var battleToAdd = new Battle()
            {
                BattleType = battleRequest.BattleType, AttackerName = ruler,
                Target = battleRequest.Target.Ruler, ResolutionTime = resolutionTime, Winner = winner,
                TimeToStartTheBattle = timeToStartTheBattle
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

        public string GetWinner(Player player, Kingdom kingdom, out string loser, out List<TroopsList> winnerLostTroops,
            out List<TroopsList> loserLostTroops)
        {
            int defenderPower = GetSumOfDefensePower(kingdom);
            int attackPower = GetSumOfAttackPower(player);
            var battleResult = attackPower - defenderPower;

            if (battleResult > 0)
            {
                loser = kingdom.Player.Username;
                TakeTroops(player.KingdomId, kingdom.KingdomId, out winnerLostTroops, out loserLostTroops);
                return player.Username;
            }

            loser = player.Username;
            TakeTroops(kingdom.Player.KingdomId, player.KingdomId, out winnerLostTroops, out loserLostTroops);
            return kingdom.Player.Username;
        }

        public void TakeTroops(int winnerKingdomId, int loserKingdomId, out List<TroopsList> winnerLostTroops,
            out List<TroopsList> loserTroopsLost)
        {
            var winner = DumDumService.GetKingdomById(winnerKingdomId);
            var loser = DumDumService.GetKingdomById(loserKingdomId);
            winnerLostTroops = new List<TroopsList>();
            var winnersTroops = GetTroopsByKingdomId(winner.KingdomId);
            var toDivideWith = winnersTroops.Count;
            float amountDivision = toDivideWith / 100f;
            var amountWinner = amountDivision * 40;
            var winList = winnersTroops.Take((int) amountWinner);
            if (winList.Count() != 0)
            {
                foreach (var troop in winList)
                {
                    var lost = new TroopsList()
                    {
                        Quantity = GetTroopsByKingdomId(winner.KingdomId)
                            .Count(t => t.TroopTypeId.Equals(troop.TroopTypeId)),
                        Type = GetTroopTypeById(troop.TroopTypeId).TroopType
                    };
                    winnerLostTroops.Add(lost);
                    DbContext.Troops.Remove(troop);
                    DbContext.SaveChanges();
                }

                winner.Troops.RemoveRange(1, (int) amountWinner);
            }
            else
            {
                var lost = new TroopsList()
                {
                    Quantity = 0,
                    Type = ""
                };
                winnerLostTroops.Add(lost);
            }

            loserTroopsLost = new List<TroopsList>();
            if (GetTroopsByKingdomId(loserKingdomId) is not null)
            {
                foreach (var troop in GetTroopsByKingdomId(loserKingdomId))
                {
                    var troopToAdd = new TroopsList()
                    {
                        Quantity = loser.Troops.Count(t => t.TroopTypeId.Equals(troop.TroopTypeId)),
                        Type = GetTroopTypeById(troop.TroopTypeId).TroopType
                    };

                    if (!loserTroopsLost.Any(t => t.Type == troopToAdd.Type))
                    {
                        loserTroopsLost.Add(troopToAdd);
                    }

                    DbContext.Troops.Remove(troop);
                    DbContext.SaveChanges();
                }
            }
        }

        public void TakeAndGiveLoot(string winner, string loser, out float goldStolen, out float foodStolen)
        {
            var kingdomOfWinner = DumDumService.GetPlayerByUsername(winner).Kingdom;
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

        private Attacker AddAttacker(string attackerName, int battleId,
            List<TroopsList> troopsLost, ResourcesStolen resourcesStolen)
        {
            var attacker = new Attacker()
                {AttackerName = attackerName, BattleId = battleId};
            var attackerToReturn = DbContext.Attackers.Add(attacker).Entity;
            DbContext.SaveChanges();
            attackerToReturn.TroopsLost = troopsLost;
            attackerToReturn.ResourcesStolen = resourcesStolen;
            DbContext.Update(attackerToReturn);
            return attackerToReturn;
        }

        private Defender AddDefender(string defenderName, int battleId, List<TroopsList> troopsLost)
        {
            var defender = new Defender() {DefenderName = defenderName, BattleId = battleId,};
            var defenderToReturn = DbContext.Defenders.Add(defender).Entity;
            defenderToReturn.TroopsLost = troopsLost;
            DbContext.SaveChanges();
            return defenderToReturn;
        }

        private TroopTypes GetTroopTypeById(int troopTypeId)
        {
            return DbContext.TroopTypes.FirstOrDefault(t => t.TroopTypeId == troopTypeId);
        }
    }
}