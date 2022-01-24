

namespace DumDum.Models.JsonEntities.Troops
{
    public class TroopsPoint
    {
        public string Ruler { get; set; }
        public string Kingdom { get; set; }
        public int Troops { get; set; }
        public double Points { get; set; }
        
        public TroopsPoint(Entities.Kingdom kingdom, double points, int troops)
        {
            Ruler = kingdom.Player.Username;
            Kingdom = kingdom.KingdomName;
            Troops = troops;
            Points = points;
        }

        public TroopsPoint(string ruler, string kingdom, int troops, double points)
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
