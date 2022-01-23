using System.Collections.Generic;

namespace DumDum.Models.Entities
{
    public class Building
    {
        public int BuildingId { get; set; }
        public string BuildingType { get; set; }
        public int Level { get; set; }
        public long StartedAt { get; set; }
        public long FinishedAt { get; set; }
        public Kingdom Kingdom { get; set; }
        public int KingdomId { get; set; }
        public List<BuildingType> BuildingTypes { get; set; }
        public int BuildingTypeId { get; set; }

    }
}
