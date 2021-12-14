using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public int KingdomId { get; set; }
        public Kingdom Kingdom { get; set; }

        public Troop CreateAxeman()
        {
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
        }


    }
}

