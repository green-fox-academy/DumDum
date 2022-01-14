using System.Threading.Tasks;

namespace DumDum.Models.JsonEntities.Kingdom
{
    public class KingdomPoints
    {
        public string Ruler { get; set; }
        public string Kingdom { get; set; }
        public double Points { get; set; }

        public KingdomPoints()
        {
                
        }
        public KingdomPoints(Entities.Kingdom kingdom, double points)
        {
            Ruler = kingdom.Player.Username;
            Kingdom = kingdom.KingdomName;
            Points = points;     
        }
    }
}
