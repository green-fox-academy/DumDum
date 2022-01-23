

namespace DumDum.Models.JsonEntities.Kingdom
{
    public class KingdomPoints
    {
        public string Ruler { get; set; }
        public string Kingdom { get; set; }
        public decimal Points { get; set; }

        public KingdomPoints()
        {
                
        }
        public KingdomPoints(Entities.Kingdom kingdom, decimal points)
        {
            Ruler = kingdom.Player.Username;
            Kingdom = kingdom.KingdomName;
            Points = points;     
        }
    }
}
