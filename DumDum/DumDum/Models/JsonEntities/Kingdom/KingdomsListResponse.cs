using System.Collections.Generic;
using DumDum.Models.Entities;

namespace DumDum.Models.JsonEntities
{
    public class KingdomsListResponse
    {
        public List<KingdomResponse> Kingdoms { get; set; }
    }
}
