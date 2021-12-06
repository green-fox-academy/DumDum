using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DumDum.Models.Entities
{
    public class Kingdom
    {
        public int KingdomId { get; set; }
        public int CoordinateX { get; set; }
        public int CoordinateY { get; set; }
        public int PlayerId { get; set; }
    }
}
