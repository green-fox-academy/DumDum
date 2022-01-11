using System.Collections.Generic;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Kingdom;

namespace DumDum.Models.JsonEntities.Resources
{
    public class ResourceResponse
    {
        public KingdomResponse Kingdom { get; set; }
        public List<ResourceList> Resources { get; set; }

        public ResourceResponse()
        {
            
        }

        public ResourceResponse(KingdomResponse kingdomResponse, List<ResourceList> resources)
        {
            Kingdom = kingdomResponse;
            Resources = resources;
        }
        
    }
}