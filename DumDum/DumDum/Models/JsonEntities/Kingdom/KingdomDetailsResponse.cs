using System.Collections.Generic;
using DumDum.Models.JsonEntities.Buildings;
using DumDum.Models.JsonEntities.Resources;
using DumDum.Models.JsonEntities.Troops;

namespace DumDum.Models.JsonEntities.Kingdom
{
    public class KingdomDetailsResponse
    {
        public KingdomResponse Kingdom { get; set; }
        public List<ResourceList> Resources { get; set; }
        public List<BuildingList> Buildings { get; set; }
        public List<TroopsResponse> Troops { get; set; }
    }
}