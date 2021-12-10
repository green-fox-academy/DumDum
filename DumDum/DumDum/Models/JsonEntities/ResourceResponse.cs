using System.Collections.Generic;
using DumDum.Models.Entities;

namespace DumDum.Models.JsonEntities
{
    public class ResourceResponse
    {
        public int KingdomId { get; set; }
        public string KingdomName { get; set; }
        public string Ruler { get; set; }
        public int Population { get; set; }
        public Location Location { get; set; }
        public List<Resource> Resources { get; set; }
    }
}