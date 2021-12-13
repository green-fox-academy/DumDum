using System.Collections.Generic;

namespace DumDum.Models.JsonEntities
{
    public class KingdomDetailsResponse
    {
        public KingdomResponse Kingdom { get; set; }
        public ResourcesResponse Resources { get; set; }
        public List<BuildingResponse> Buildings { get; set; }
        public List<TroopsResponse> Troops { get; set; }
        public int StatusCode { get; set; }
    }
}