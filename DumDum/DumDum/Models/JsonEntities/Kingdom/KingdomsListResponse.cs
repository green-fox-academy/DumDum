using System.Collections.Generic;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Kingdom;

namespace DumDum.Models.JsonEntities
{
    public class KingdomsListResponse
    {
        public List<KingdomResponse> Kingdoms { get; set; }
    }
}
