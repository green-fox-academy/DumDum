using System.ComponentModel.DataAnnotations.Schema;
using DumDum.Models.JsonEntities.Battles;

namespace DumDum.Models.Entities
{
    public class Battle
    {
        public int BattleId { get; set; }
        public string AttackerName { get; set; }
        [NotMapped]public Attacker Attacker { get; set; }
        [NotMapped]public Defender Defender { get; set; }
        public string Target { get; set; }
        public string BattleType { get; set; }
        [NotMapped]public Kingdom Kingdom { get; set; }
        public long ResolutionTime { get; set; }
        public string Winner { get; set; }
        public long TimeToStartTheBattle { get; set; }
        
        
    }
}