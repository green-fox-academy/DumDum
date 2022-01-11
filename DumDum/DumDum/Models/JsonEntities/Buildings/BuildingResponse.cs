using System.Collections.Generic;
using System.Threading.Tasks;
using DumDum.Models.JsonEntities.Kingdom;

namespace DumDum.Models.JsonEntities.Buildings
{
    public class BuildingResponse
    {
        public Task<KingdomResponse> Kingdom { get; set; }
        public Task<List<BuildingList>> Buildings { get; set; }

        public BuildingResponse(Task<KingdomResponse> kingdomResponse, Task<List<BuildingList>> buildings)
        {
            Kingdom = kingdomResponse;
            Buildings = buildings;
        }
    }
    
}
