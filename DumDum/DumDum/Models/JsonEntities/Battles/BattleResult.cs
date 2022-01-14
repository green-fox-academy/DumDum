namespace DumDum.Models.JsonEntities.Battles
{
    public class BattleResult
    {
        public int BattleId { get; set; }
        public long ResolutionTime { get; set; }
        public string BattleType { get; set; }
        public string Winner { get; set; }
        public Attacker Attacker { get; set; }
        public Defender Defender { get; set; }

        public BattleResult()
        {
            
        }

        public BattleResult(int battleId, long resolutionTime, string battleType, string winner, Attacker attacker, Defender defender )
        {
                BattleId = battleId;
                ResolutionTime = resolutionTime;
                Winner = winner;
                Attacker = attacker;
                Defender = defender;
        }

    }
}