namespace DumDum.Models.JsonEntities
{
    public class TroopsResponse
    {
        public int TroopId { get; set; }
        public string TroopType { get; set; }
        public int Level { get; set; }
        public int HP { get; set; }
        public double Attack { get; set; }
        public double Defence { get; set; }
        public int StartedAt { get; set; }
        public int FinishedAt { get; set; }
    }
}
