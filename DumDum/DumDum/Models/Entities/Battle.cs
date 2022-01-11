using System.ComponentModel.DataAnnotations.Schema;
using DumDum.Models.JsonEntities.Battles;

namespace DumDum.Models.Entities
{
    public class Battle
    {
        public int BattleId { get; set; }
        public int AttackerId { get; set; }
        public int DefenderId { get; set; }
        [NotMapped]public string Target { get; set; }
        public string BattleType { get; set; }
        [NotMapped]public Kingdom Kingdom { get; set; }
        public long ResolutionTime { get; set; }
        public int WinnerPlayerId { get; set; }
        public long TimeToStartTheBattle { get; set; }
        public int GoldStolen { get; set; }
        public int FoodStolen { get; set; }
        
        
    }
}