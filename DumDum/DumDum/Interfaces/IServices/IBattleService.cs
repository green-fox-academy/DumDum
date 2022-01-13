using System.Collections.Generic;
using System.Threading.Tasks;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Battles;

namespace DumDum.Interfaces.IServices
{
    public interface IBattleService
    {
        Task<(BattleResult, int)> GetBattleResult(string authorization, int attackerKingdomId, int battleId);
        Task<(BattleResponse, int)> MakeBattle(string authorization, int attackerKingdomId, BattleRequest battleRequest);
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
