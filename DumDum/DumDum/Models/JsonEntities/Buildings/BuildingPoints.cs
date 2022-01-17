using DumDum.Models.JsonEntities.Kingdom;

namespace DumDum.Models.JsonEntities.Buildings
{
    public class BuildingPoints
    {
        public string Ruler { get; set; }
        public string Kingdom { get; set; }
        public int Buildings { get; set; }
        public int Points { get; set; }
        
        public BuildingPoints(Entities.Kingdom kingdom, int buildings, int points)
        {
            Ruler = kingdom.Player.Username;
            Kingdom = kingdom.KingdomName;
            Buildings = buildings;
            Points = points;
        }

        public BuildingPoints(string rule, string kingdom, int buildings, int points)
        {
            Ruler = rule;
            Kingdom = kingdom;
            Buildings = buildings;
            Points = points;
        }
        public BuildingPoints()
        {
        }
    }
}
