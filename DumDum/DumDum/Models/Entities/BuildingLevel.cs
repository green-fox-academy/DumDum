namespace DumDum.Models.Entities
{
    public class BuildingLevel
    {
        public int BuildingLevelId { get; set; }
        public int LevelNumber { get; set; }
        public int Cost { get; set; }
        public long ConstTime { get; set; }
        public long ResearchTime { get; set; }
        public int MaxStorage { get; set; }
        public int Defense { get; set; }
        public int Production { get; set; }
        public int Consumption { get; set; }
        public decimal DefBoost { get; set; }
        public long Axemen { get; set; }
        public long Phalanx { get; set; }
        public long Knights { get; set; }
        public long Catapult { get; set; }
        public long TheSpy { get; set; }
        public long Senator { get; set; }
        public BuildingType BuildingType { get; set; }
    }
}