using DumDum.Interfaces;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Battles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DumDum.Services
{
    public class BattleService : IBattleService
    {
        private IUnitOfWork UnitOfWork { get; set; }
        private IAuthenticateService AuthenticateService { get; set; }
        private IDumDumService DumDumService { get; set; }

        public BattleService(IUnitOfWork unitOfWork, IAuthenticateService authService, IDumDumService dumService)
        {
            UnitOfWork = unitOfWork;
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

                var battle = UnitOfWork.Battles.GetBattleById(battleId);
                var attackerTroopsLost = UnitOfWork.TroopsLost.GetListOfTroopsLost(battle.AttackerId, battleId);
                var defenderTroopsLost = UnitOfWork.TroopsLost.GetListOfTroopsLost(battle.DefenderId, battleId);
                var attacker = new Attacker()
                    {ResourcesStolen = new ResourcesStolen() {Food = battle.FoodStolen, Gold = battle.GoldStolen}};
                var defender = new Defender() { };
                var troopAttackerList = new List<TroopsList>();
                var troopDefenderList = new List<TroopsList>();

                foreach (var troop in attackerTroopsLost)
                {
                    var troopToAddList = new TroopsList()
                        {Quantity = troop.Quantity, Type = UnitOfWork.Battles.GetTroopTypeById(troop.Type).TroopType};
                    troopAttackerList.Add(troopToAddList);
                }

                foreach (var troop in defenderTroopsLost)
                {
                    var troopToAddList = new TroopsList()
                        {Quantity = troop.Quantity, Type = UnitOfWork.Battles.GetTroopTypeById(troop.Type).TroopType};
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
                    var troopToUpdate = UnitOfWork.TroopsLost.TroopToUpdate(troop.TroopLostId, winner);

                    if (troopToUpdate is not null)
                    {
                        troopToUpdate.BattleId = battle.BattleId;
                        UnitOfWork.TroopsLost.Update(troopToUpdate);
                        UnitOfWork.Complete();
                    }
                }

                foreach (var troop in loserLostTroops)
                {
                    var troopToUpdate = UnitOfWork.TroopsLost.TroopToUpdate(troop.TroopLostId,
                        DumDumService.GetPlayerByUsername(loser).PlayerId);

                    if (troopToUpdate is not null)
                    {
                        troopToUpdate.BattleId = battle.BattleId;
                        UnitOfWork.TroopsLost.Update(troopToUpdate);
                        UnitOfWork.Complete();
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

            var battle = UnitOfWork.Battles.AddBattle(battleToAdd);
            UnitOfWork.Complete();
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
            if (UnitOfWork.Battles.GetTroopsByKingdomId(kingdom.KingdomId) is null)
            {
                return 0;
            }

            var minSpeed = UnitOfWork.Battles.MinSpeed(kingdomId);

            return minSpeed;
        }

        public int GetSumOfAttackPower(Player player)
        {
            if (UnitOfWork.Battles.GetTroopsByKingdomId(player.KingdomId) is null)
            {
                return 0;
            }

            var attackPower = UnitOfWork.Battles.SumOfAttackPower(player.KingdomId);

            return (int) attackPower;
        }


        public int GetSumOfDefensePower(Kingdom kingdom)
        {
            if (UnitOfWork.Battles.GetTroopsByKingdomId(kingdom.KingdomId) is null)
            {
                return 0;
            }

            var defensePower = UnitOfWork.Battles.GetDefensePower(kingdom.KingdomId);

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
            var winnersTroops = UnitOfWork.Battles.GetTroopsByKingdomId(winner.KingdomId);
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
                        Quantity = UnitOfWork.Battles.GetTroopsByKingdomId(winner.KingdomId)
                            .Count(t => t.TroopTypeId.Equals(troop.TroopTypeId)),
                        Type = troop.TroopTypeId, PlayerId = winner.PlayerId
                    };

                    var lostTroop = UnitOfWork.TroopsLost.AddTroopsLost(lost);
                    winnerLostTroops.Add(lostTroop);
                    UnitOfWork.Troops.Remove(troop);
                    UnitOfWork.Complete();
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
            if (UnitOfWork.Battles.GetTroopsByKingdomId(loserKingdomId) is not null)
            {
                foreach (var troop in UnitOfWork.Battles.GetTroopsByKingdomId(loserKingdomId))
                {
                    var troopToAdd = new TroopsLost()
                    {
                        Quantity = loser.Troops.Count(t => t.TroopTypeId.Equals(troop.TroopTypeId)),
                        Type = troop.TroopTypeId, PlayerId = loser.PlayerId
                    };

                    if (!loserTroopsLost.Any(t => t.Type.Equals(troopToAdd.Type)))
                    {
                        var lostTroop = UnitOfWork.TroopsLost.AddTroopsLost(troopToAdd);
                        loserTroopsLost.Add(lostTroop);
                    }

                    UnitOfWork.Troops.Remove(troop);
                    UnitOfWork.Complete();
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
    }
}