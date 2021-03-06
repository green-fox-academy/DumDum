
namespace DumDum.Models.Entities
{
    public class BuildingLevel
    {
        public int BuildingLevelId { get; set; }
        public int LevelNumber { get; set; }
        public int Cost { get; set; }
        public long ConstTime { get; set; }
        public int Production { get; set; }
        public int Consumption { get; set; }
        public BuildingType BuildingType { get; set; }
    }
}