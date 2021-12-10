using DumDum.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DumDum.Models.JsonEntities
{
    public class KingdomResponse
    {
        public int KingdomId { get; set; }
        public string KingdomName { get; set; }
        public string Ruler { get; set; }
        public int Population { get; set; }
        public Location Location { get; set; }
    }
}
