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
        Task<Battle> AddBattle(BattleRequest battleRequest, int attackerId, long resolutionTime, int winnerId,
            long timeToStartTheBattle, int foodStolen, int goldStolen);
        Task<long> ResolutionTimeCount(int coordinateX, int coordinateY, decimal minSpeed);
        Task<decimal> GetMinSpeed(int kingdomId);
        Task<int> GetSumOfAttackPower(Player player);
        Task<int> GetSumOfDefensePower(Kingdom kingdom);
        Task<(int, string, List<TroopsLost>, List<TroopsLost>)> GetWinner(Player player, Kingdom kingdom);
        Task<(List<TroopsLost>, List<TroopsLost>)> TakeTroops(int winnerKingdomId, int loserKingdomId);
        Task<(float, float)> TakeAndGiveLoot(int winnerId,string loser);
    }
}
