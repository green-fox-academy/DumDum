namespace DumDum.Models.JsonEntities.Buildings
{
    public class BuildingList
    {
        public int BuildingId { get; set; }
        public string BuildingType { get; set; }
        public int Level { get; set; }
        public int Hp { get; set; }
        public long StartedAt { get; set; }
        public long FinishedAt { get; set; }
        public int Production { get; set; }
        public int Consumption { get; set; }
    }
}
