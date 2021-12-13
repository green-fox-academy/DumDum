using DumDum.Models.Entities;
using System;
using System.Collections.Generic;

namespace DumDum.Models.JsonEntities
{
    public class KingdomsListResponse
    {
        public int KingdomId { get; set; }
        public string KingdomName { get; set; }
        public string Ruler { get; set; }
        public int Population { get; set; }
        public Location Location { get; set; }
    }
}
