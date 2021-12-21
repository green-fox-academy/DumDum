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
        public long CreatedTimeAxemen { get; set; }
        public long CreatedTimePhalanx { get; set; }
        public long CreatedTimeKnights { get; set; }
        public long CreatedTimeCatapult { get; set; }
        public long CreatedTimeTheSpy { get; set; }
        public long CreatedTimeSenator { get; set; }
        public BuildingType BuildingType { get; set; }
    }
}