using System.Collections.Generic;
using DumDum.Models.JsonEntities.Buildings;

namespace DumDum.Models.JsonEntities
{
    public class KingdomDetailsResponse
    {
        public KingdomResponse Kingdom { get; set; }
        public List<ResourceList> Resources { get; set; }
        public List<BuildingList> Buildings { get; set; }
        public List<TroopsResponse> Troops { get; set; }
    }
}