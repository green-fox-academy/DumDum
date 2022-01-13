using DumDum.Models.Entities;

namespace DumDum.Models.JsonEntities.Resources
{
    public class ResourceList
    {
        public int ResourceId { get; set; }
        public string ResourceType { get; set; }
        public int Amount { get; set; }
        public int Generation { get; set; }
        public long UpdatedAt { get; set; }

        public ResourceList(Resource resource)
        {
            ResourceType = resource.ResourceType;
            Amount = resource.Amount;
            Generation = resource.Generation;
            UpdatedAt = resource.UpdatedAt;
        }
    }
}