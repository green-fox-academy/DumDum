using System.Collections.Generic;

namespace DumDum.Models.Entities
{
    public class Kingdom
    {
        public int KingdomId { get; set; }
        public string KingdomName { get; set; }
        public int CoordinateX { get; set; }
        public int CoordinateY { get; set; }
        public Player Player { get; set; }
        public int PlayerId { get; set; }
        public List<Troop> Troops { get; set; }
        public List<Resource> Resources { get; set; }
        public List<Building> Buildings { get; set; }

        public Kingdom(Kingdom kingdom)
        {
            KingdomId = kingdom.KingdomId;
            KingdomName = kingdom.KingdomName;
            CoordinateX = kingdom.CoordinateX;
            CoordinateY = kingdom.CoordinateY;
            Player = kingdom.Player;
            PlayerId = kingdom.PlayerId;
            Troops = kingdom.Troops;
            Resources = kingdom.Resources;
            Buildings = kingdom.Buildings;
        }

        public Kingdom()
        {
            
        }
    }
}
