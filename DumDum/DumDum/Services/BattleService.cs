using DumDum.Interfaces.IServices;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Battles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DumDum.Interfaces.IRepositories;

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

        public async Task<(BattleResult, int)> GetBattleResult(string authorization, int attackerKingdomId, int battleId)
        {
            if (authorization != "")
            {
                AuthRequest request = new AuthRequest();
                request.Token = authorization.Remove(0, 7);
                var player = await AuthenticateService.GetUserInfo(request);

                if (player == null)
                {
                    return (new BattleResult(), 401);
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
                
                return (new BattleResult(battle.BattleId, battle.ResolutionTime, battle.BattleType,
                    DumDumService.GetPlayerById(battle.WinnerPlayerId).Result.Username, attacker, defender), 200);
            }
            return new (new BattleResult(), 401);
        }

        public async Task<(BattleResponse, int)> MakeBattle(string authorization, int attackerKingdomId, BattleRequest battleRequest)
        {
            if (authorization == "")
            {
                return (new BattleResponse(), 401);
            }

            AuthRequest request = new AuthRequest();
            request.Token = authorization.Remove(0, 7);
            var player = await AuthenticateService.GetUserInfo(request);

            if (player != null)
            {
                var kingdom = await DumDumService.GetKingdomById(battleRequest.Target.KingdomId);
                var minSpeed = await GetMinSpeed(attackerKingdomId);
                var attacker = await DumDumService.GetPlayerByUsername(player.Ruler);
                var resolutionTime = await ResolutionTimeCount(battleRequest.Target.Location.CoordinateX,
                    battleRequest.Target.Location.CoordinateY, minSpeed);
                
                var timeToStartTheBattle =
                    (int) (long) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)))
                    .TotalSeconds + resolutionTime;
                
                var winner = await GetWinner(attacker, kingdom);
                var winnerLostTroops = winner.Item3;
                var loserLostTroops = winner.Item4;
                string loser = winner.Item2;
                var goldAndFood = await TakeAndGiveLoot(winner.Item1, loser);
                
                var battle = await AddBattle(battleRequest, attacker.PlayerId, resolutionTime, winner.Item1,
                    timeToStartTheBattle, (int)goldAndFood.Item2, (int)goldAndFood.Item1);

                foreach (var troop in winnerLostTroops)
                {
                    var troopToUpdate = UnitOfWork.TroopsLost.TroopToUpdate(troop.TroopLostId, winner.Item1);

                    if (troopToUpdate is not null)
                    {
                        troopToUpdate.BattleId = battle.BattleId;
                        UnitOfWork.TroopsLost.Update(troopToUpdate);
                        UnitOfWork.Complete();
                    }
                }

                foreach (var troop in loserLostTroops)
                {
                    var playerByUserName = await DumDumService.GetPlayerByUsername(loser);
                    var troopToUpdate = UnitOfWork.TroopsLost.TroopToUpdate(troop.TroopLostId, playerByUserName.PlayerId);

                    if (troopToUpdate is not null)
                    {
                        troopToUpdate.BattleId = battle.BattleId;
                        UnitOfWork.TroopsLost.Update(troopToUpdate);
                        UnitOfWork.Complete();
                    }
                }
                return (new BattleResponse(battle.BattleId, battle.ResolutionTime), 200);
                
            }
            return (new BattleResponse(), 401);
        }

        public async Task<Battle> AddBattle(BattleRequest battleRequest, int attackerId, long resolutionTime, int winnerId,
            long timeToStartTheBattle, int foodStolen, int goldStolen)
        {
            var battleToAdd = new Battle()
            {
                BattleType = battleRequest.BattleType, AttackerId = attackerId,
                DefenderId = DumDumService.GetPlayerByUsername(battleRequest.Target.Ruler).Result.PlayerId,
                ResolutionTime = resolutionTime, WinnerPlayerId = winnerId,
                TimeToStartTheBattle = timeToStartTheBattle, FoodStolen = foodStolen, GoldStolen = goldStolen
            };

            var battle = UnitOfWork.Battles.AddBattle(battleToAdd);
            UnitOfWork.Complete();
            return battle;
        }

        public async Task<long> ResolutionTimeCount(int coordinateX, int coordinateY, double minSpeed)
        {
            double newCoordinateX = Convert.ToDouble(coordinateX * coordinateX);
            double newCoordinateY = Convert.ToDouble(coordinateY * coordinateY);
            double toSquare = newCoordinateX + newCoordinateY;

            return Convert.ToInt64(Math.Sqrt(toSquare) * minSpeed);
        }

        public async Task<double> GetMinSpeed(int kingdomId)
        {
            var kingdom = await DumDumService.GetKingdomById(kingdomId);
            if (UnitOfWork.Battles.GetTroopsByKingdomId(kingdom.KingdomId) is null)
            {
                return 0;
            }

            var minSpeed = UnitOfWork.Battles.MinSpeed(kingdomId);

            return minSpeed;
        }

        public async Task<int> GetSumOfAttackPower(Player player)
        {
            if (UnitOfWork.Battles.GetTroopsByKingdomId(player.KingdomId) is null)
            {
                return 0;
            }

            var attackPower = UnitOfWork.Battles.SumOfAttackPower(player.KingdomId);

            return (int) attackPower;
        }


        public async Task<int> GetSumOfDefensePower(Kingdom kingdom)
        {
            if (UnitOfWork.Battles.GetTroopsByKingdomId(kingdom.KingdomId) is null)
            {
                return 0;
            }

            var defensePower = UnitOfWork.Battles.GetDefensePower(kingdom.KingdomId);

            return (int) defensePower;
        }

        public async Task<(int, string, List<TroopsLost>, List<TroopsLost>)> GetWinner(Player player, Kingdom kingdom)
        {
            int defenderPower = await GetSumOfDefensePower(kingdom);
            int attackPower = await GetSumOfAttackPower(player);
            var battleResult = attackPower - defenderPower;

            if (battleResult > 0)
            {
                var result = await TakeTroops(player.KingdomId, kingdom.KingdomId);
                return (player.PlayerId, kingdom.Player.Username, result.Item2, result.Item1 );
            }
            var result1 = await TakeTroops(kingdom.Player.KingdomId, player.KingdomId);
            return (player.PlayerId, player.Username, result1.Item2, result1.Item1);
        }

        public async Task<(List<TroopsLost>, List<TroopsLost>)> TakeTroops(int winnerKingdomId, int loserKingdomId)
        {
            var winner =  await DumDumService.GetKingdomById(winnerKingdomId);
            var loser = await DumDumService.GetKingdomById(loserKingdomId);
            var winnerLostTroops = new List<TroopsLost>();
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

            var loserTroopsLost = new List<TroopsLost>();
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

            return (loserTroopsLost, winnerLostTroops);
        }

        public async Task<(float, float)> TakeAndGiveLoot(int winnerId, string loser)
        {
            var kingdomOfWinner = await DumDumService.GetPlayerById(winnerId);
            var kingdomOfLoser = await DumDumService.GetPlayerByUsername(loser);
            var amountOfGold = await DumDumService.GetGoldAmountOfKingdom(kingdomOfLoser.KingdomId);
            float amountOfGoldToTakeOrGive = amountOfGold / 100f;
            float goldStolen = amountOfGoldToTakeOrGive * 20;
            await DumDumService.TakeGold(kingdomOfLoser.KingdomId, (int) goldStolen);
            float amountOfFood = DumDumService.GetFoodAmountOfKingdom(kingdomOfLoser.KingdomId).Result;
            var amountOfFoodToTakeOrGive = amountOfFood / 100;
            float foodStolen = amountOfFoodToTakeOrGive * 20;
            await DumDumService.TakeFood(kingdomOfLoser.KingdomId, (int) foodStolen);
            await DumDumService.GiveGold(kingdomOfWinner.KingdomId, (int) goldStolen);
            await DumDumService.GiveFood(kingdomOfWinner.KingdomId, (int) foodStolen);
            return (goldStolen, foodStolen);
        }
    }
}