namespace DumDum.Models.Entities
{
    public class Troop
    {
        public int TroopId { get; set; }
        public string TroopType { get; set; }
        public int Level { get; set; }
        public int HP { get; set; }
        public double Attack { get; set; }
        public double Defence { get; set; }
        public double Speed { get; set; }
        public int StartedAt { get; set; }
        public int FinishedAt { get; set; }
        public int Consumption { get; set; }
        public int CarryCap { get; set; }
        public int Cost { get; set; }
        public int SpecialSkills { get; set; }
        public int KingdomId { get; set; }
        public Kingdom Kingdom { get; set; }
    }
}

