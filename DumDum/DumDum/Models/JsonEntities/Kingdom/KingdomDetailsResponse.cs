using System.Collections.Generic;
using System.Threading.Tasks;
using DumDum.Models.JsonEntities.Buildings;
using DumDum.Models.JsonEntities.Resources;
using DumDum.Models.JsonEntities.Troops;


namespace DumDum.Models.JsonEntities.Kingdom
{
    public class KingdomDetailsResponse
    {
        public Task<KingdomResponse> Kingdom { get; set; }
        public List<ResourceList> Resources { get; set; }
        public Task<List<BuildingList>> Buildings { get; set; }
        public List<TroopsResponse> Troops { get; set; }

        public KingdomDetailsResponse(Task<KingdomResponse> kingdom, List<ResourceList> resources, Task<List<BuildingList>> buildings, List<TroopsResponse> troops)
        {
            Kingdom = kingdom;
            Resources = resources;
            Buildings = buildings;
            Troops = troops;
        }
    }
}