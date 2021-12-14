using System.Collections.Generic;

namespace DumDum.Models.JsonEntities.Buildings
{
    public class BuildingResponse
    {
        public KingdomResponse Kingdom { get; set; }
        public List<BuildingList> Buildings { get; set; }
    }
}
