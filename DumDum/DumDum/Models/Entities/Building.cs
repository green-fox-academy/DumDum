namespace DumDum.Models.Entities
{
    public class Building
    {
        public int BuildingId { get; set; }
        public string BuildingType { get; set; }
        public int Level { get; set; }
        public int Hp { get; set; }
        public long StartedAt { get; set; }
        public long FinishedAt { get; set; }
        public Kingdom Kingdom { get; set; }
        public int KingdomId { get; set; }
        public int Production { get; set; }
        public int Consumption { get; set; }
        public int Defense { get; set; }
        public int MaxStorage { get; set; }
    }
}
