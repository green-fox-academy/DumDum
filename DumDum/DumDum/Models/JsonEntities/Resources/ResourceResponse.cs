using System.Collections.Generic;
using DumDum.Models.Entities;

namespace DumDum.Models.JsonEntities
{
    public class ResourceResponse
    {
        public KingdomResponse Kingdom { get; set; }
        public List<ResourceList> Resources { get; set; }
    }
}