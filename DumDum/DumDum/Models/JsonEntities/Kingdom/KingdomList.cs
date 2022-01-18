using System.Collections.Generic;
using DumDum.Models.Entities;

namespace DumDum.Models.JsonEntities.Kingdom
{
    public class KingdomList
    {
        public int KingdomId { get; set; }
        public string KingdomName { get; set; }
        public int CoordinateX { get; set; }
        public int CoordinateY { get; set; }
        public Entities.Player Player { get; set; }
        public int PlayerId { get; set; }
        public List<Troop> Troops { get; set; }
        public List<Resource> Resources { get; set; }
        public List<Building> Buildings { get; set; }

        
    }
}