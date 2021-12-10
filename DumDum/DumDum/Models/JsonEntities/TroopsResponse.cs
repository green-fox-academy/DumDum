using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DumDum.Models.JsonEntities
{
    public class TroopsResponse
    {
        public int TroopId { get; set; }
        public string TroopType { get; set; }
        public int Level { get; set; }
        public int HP { get; set; }
        public int Attack { get; set; }
        public int Defence { get; set; }
        public int StartedAt { get; set; }
        public int FinishedAt { get; set; }

    }
    
}
