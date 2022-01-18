using System.Collections.Generic;
using DumDum.Models.JsonEntities.Kingdom;

namespace DumDum.Models.JsonEntities.Troops
{
    public class GetTroopsResponse
    {
        public KingdomResponse Kingdom { get; set; }
        public List<TroopsResponse> Troops { get; set; }

        public GetTroopsResponse(KingdomResponse kingdom, List<TroopsResponse> troops)
        {
            Kingdom = kingdom;
            Troops = troops;
        }
    }
}