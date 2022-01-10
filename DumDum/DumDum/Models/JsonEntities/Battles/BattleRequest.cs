using System.Collections.Generic;

namespace DumDum.Models.JsonEntities.Battles
{
    public class BattleRequest
    {
        public Target Target { get; set; }
        public string BattleType { get; set; }
        public List<TroopsList> Troops { get; set; }
    }
}