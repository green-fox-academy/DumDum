using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DumDum.Models.Entities
{
    public class Building
    {
        public int BuildingId { get; set; }
        public string BuildingType { get; set; }
        public int Level { get; set; }
        public long StartedAt { get; set; }
        public long FinishedAt { get; set; }
        public Kingdom Kingdom { get; set; }
        public int KingdomId { get; set; }

        public Building()
        {
            Level = 1;
        }
    }
}
