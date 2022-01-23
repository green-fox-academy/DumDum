using System.Linq;
using DumDum.Repository;

namespace DumDum.Models.JsonEntities.Troops
{
    public class TroopsPoint
    {
        public string Ruler { get; set; }
        public string Kingdom { get; set; }
        public int Troops { get; set; }
        public decimal Points { get; set; }
        
        public TroopsPoint(Entities.Kingdom kingdom, decimal points, int troops)
        {
            Ruler = kingdom.Player.Username;
            Kingdom = kingdom.KingdomName;
            Troops = troops;
            Points = points;
        }

        public TroopsPoint(string ruler, string kingdom, int troops, decimal points)
        {
            Ruler = ruler;
            Kingdom = kingdom;
            Troops = troops;
            Points = points;
        }
        public TroopsPoint()
        {
        }
    }
}
