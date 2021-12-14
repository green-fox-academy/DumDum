namespace DumDum.Models.Entities
{
    public class Troop
    {
        public int TroopId { get; set; }
        public string TroopType { get; set; }
        public int Level { get; set; }
        public int HP { get; set; }
        public int Attack { get; set; }
        public int Defence { get; set; }
        public double Speed { get; set; }
        public int StartedAt { get; set; }
        public int FinishedAt { get; set; }
        public int Consumption { get; set; }
        public int CarryCap { get; set; }
        public int Cost { get; set; }
        public int SpecialSkills { get; set; }
        public int KingdomId { get; set; }
        public Kingdom Kingdom { get; set; }

        public Troop CreateTroop(string troopType)
        {
            switch (troopType)
            {
                case "axeman":
                    return new()
                    {
                        TroopType = "axeman",
                        Level = 1,
                        HP = 1,
                        Attack = 8,
                        Defence = 5,
                        CarryCap = 30,
                        Consumption = 1,
                        Speed = 1,
                        Cost = 20
                    };
                    break;

                case "phalanx":
                    return new()
                    {
                        TroopType = "phalanx",
                        Level = 1,
                        HP = 1,
                        Attack = 5,
                        Defence = 8,
                        CarryCap = 30,
                        Consumption = 1,
                        Speed = 0.8,
                        Cost = 20
                    };

                case "knight":
                    return new()
                    {
                        TroopType = "knight",
                        Level = 1,
                        HP = 1,
                        Attack = 15,
                        Defence = 10,
                        CarryCap = 50,
                        Consumption = 2,
                        Speed = 1.6,
                        Cost = 50
                    };

                case "spy":
                    return new()
                    {
                        TroopType = "spy",
                        Level = 1,
                        HP = 1,
                        Attack = 4,
                        Defence = 3,
                        CarryCap = 0,
                        Consumption = 2,
                        Speed = 2.5,
                        Cost = 60
                    };

                case "senator":
                    return new()
                    {
                        TroopType = "senator",
                        Level = 1,
                        HP = 1,
                        Attack = 0,
                        Defence = 0,
                        CarryCap = 0,
                        Consumption = 5,
                        Speed = 0.5,
                        SpecialSkills = 25,
                        Cost = 1800
                    };

                default:
                    return new();
            }

        }


    }
}

