using DumDum.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DumDum.Models.JsonEntities.Kingdom
{
    public class KingdomResponse
    {
        public int KingdomId { get; set; }
        public string KingdomName { get; set; }
        public string Ruler { get; set; }
        public int Population { get; set; }
        public Location Location { get; set; }

        public KingdomResponse()
        {
                
        }

        public KingdomResponse(Entities.Kingdom kingdom, Entities.Player player, Location locations)
        {
            KingdomId = kingdom.KingdomId;
            KingdomName = kingdom.KingdomName;
            Ruler = player.Username;
            Population = 0;
            Location = locations;
        }

        public KingdomResponse(Entities.Kingdom kingdom)
        {
            KingdomId = kingdom.KingdomId;
            KingdomName = kingdom.KingdomName;
            Ruler = kingdom.Player.Username;
            Location = new Location()
            {
                CoordinateX = kingdom.CoordinateX,
                CoordinateY = kingdom.CoordinateY,
            };
        }
    }
}
