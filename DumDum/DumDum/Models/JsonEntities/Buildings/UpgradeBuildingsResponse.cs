namespace DumDum.Models.JsonEntities.Buildings
{
    public class UpgradeBuildingsResponse
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int Level { get; set; }
        public int Hp { get; set; }
        public long StartedAt { get; set; }
        public long FinishedAt { get; set; }
    }
}