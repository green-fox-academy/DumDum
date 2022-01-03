using System.Collections.Generic;

namespace DumDum.Models.Entities
{
    public class BuildingType
    {
        public int BuildingTypeId { get; set; }
        public string BuildingTypeName { get; set; }
        public int BuildingLevelId { get; set; }
        public List<BuildingLevel> BuildingLevels { get; set; }
        public BuildingLevel BuildingLevel { get; set; }
        public Building Building { get; set; }
    }
}