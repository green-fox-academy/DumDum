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

    }
}