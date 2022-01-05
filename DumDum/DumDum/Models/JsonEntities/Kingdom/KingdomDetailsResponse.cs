using System.Collections.Generic;
using DumDum.Models.JsonEntities.Buildings;
using DumDum.Models.JsonEntities.Resources;
using DumDum.Models.JsonEntities.Troops;
using DumDum.Services;

namespace DumDum.Models.JsonEntities.Kingdom
{
    public class KingdomDetailsResponse
    {
        public KingdomResponse Kingdom { get; set; }
        public List<ResourceList> Resources { get; set; }
        public List<BuildingList> Buildings { get; set; }
        public List<TroopsResponse> Troops { get; set; }

        public KingdomDetailsResponse(KingdomResponse kingdom, List<ResourceList> resources, List<BuildingList> buildings, List<TroopsResponse> troops)
        {
            Kingdom = kingdom;
            Resources = resources;
            Buildings = buildings;
            Troops = troops;
        }
    }
}