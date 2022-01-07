using System.Collections.Generic;
using System.Threading.Tasks;
using DumDum.Models.JsonEntities.Kingdom;

namespace DumDum.Models.JsonEntities.Buildings
{
    public class BuildingResponse
    {
        public KingdomResponse Kingdom { get; set; }
        public Task<List<BuildingList>> Buildings { get; set; }

        public BuildingResponse(KingdomResponse kingdomResponse, Task<List<BuildingList>> buildings)
        {
            Kingdom = kingdomResponse;
            Buildings = buildings;
        }
    }
    
}
