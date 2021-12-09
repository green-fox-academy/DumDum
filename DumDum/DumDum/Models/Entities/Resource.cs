using System;

namespace DumDum.Models.Entities
{
    public class Resource
    {
        public int ResourceId { get; set; }
        public string ResourceType { get; set; }
        public int Amount { get; set; }
        public int Generation { get; set; }
        public long UpdatedAt { get; set; }
        public Kingdom Kingdom { get; set; }
        public int KingdomId { get; set; }

    }
}