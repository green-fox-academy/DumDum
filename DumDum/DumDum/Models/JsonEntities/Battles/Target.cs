using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Kingdom;

namespace DumDum.Models.JsonEntities.Battles
{
    public class Target
    {
        public int KingdomId { get; set; }
        public string KingdomName { get; set; }
        public string Ruler { get; set; }
        public Location Location { get; set; }
    }
}