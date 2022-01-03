using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DumDum.Models.JsonEntities.Kingdom
{
    public class KingdomRegistrationRequest
    {
        public int CoordinateX { get; set; }
        public int CoordinateY { get; set; }
        public int KingdomId { get; set; }
    }
}
