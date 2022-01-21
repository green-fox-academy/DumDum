using System.Collections.Generic;

namespace DumDum.Models.JsonEntities.Battles
{
    public class Attacker
    {
        public ResourcesStolen ResourcesStolen { get; set; }
        public List<TroopsList> TroopsLost { get; set; }
    }
}