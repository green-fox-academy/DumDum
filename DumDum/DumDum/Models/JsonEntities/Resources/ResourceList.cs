namespace DumDum.Models.JsonEntities.Resources
{
    public class ResourceList
    {
        public int ResourceId { get; set; }
        public string ResourceType { get; set; }
        public int Amount { get; set; }
        public int Generation { get; set; }
        public long UpdatedAt { get; set; }
    }
}