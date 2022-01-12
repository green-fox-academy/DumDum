using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Battles;
using DumDum.Models.JsonEntities.Kingdom;
using System.Collections.Generic;

namespace DumDum.Interfaces
{
    public interface IBattleService
    {
        BattleResult GetBattleResult(string authorization, int attackerKingdomId, int battleId, out int statusCode);
        BattleResponse MakeBattle(string authorization, int attackerKingdomId, BattleRequest battleRequest, out int statusCode);
        Battle AddBattle(BattleRequest battleRequest, int attackerId, long resolutionTime, int winnerId,
            long timeToStartTheBattle, int foodStolen, int goldStolen);
        long ResolutionTimeCount(int coordinateX, int coordinateY, double minSpeed);
        double GetMinSpeed(int kingdomId);
        int GetSumOfAttackPower(Player player);
        int GetSumOfDefensePower(Kingdom kingdom);
        int GetWinner(Player player, Kingdom kingdom, out string loser, out List<TroopsLost> winnerLostTroops,
            out List<TroopsLost> loserLostTroops);
        void TakeTroops(int winnerKingdomId, int loserKingdomId, out List<TroopsLost> winnerLostTroops,
            out List<TroopsLost> loserTroopsLost);
        void TakeAndGiveLoot(int winnerId, string loser, out float goldStolen, out float foodStolen);
    }
}
