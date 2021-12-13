using DumDum.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DumDum.Models.JsonEntities.Buildings
{
    public class BuildingResponse
    {
        public KingdomResponse Kingdom { get; set; }
        public List<BuildingList> Buildings { get; set; }
    }
}
