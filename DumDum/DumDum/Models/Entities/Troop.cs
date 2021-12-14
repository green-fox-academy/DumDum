using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DumDum.Models.Entities
{
    public  abstract class Troop
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
        public int KingdomId { get; set; }
        public Kingdom Kingdom { get; set; }
    }
}
