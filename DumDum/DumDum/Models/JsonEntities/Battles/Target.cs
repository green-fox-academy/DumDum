using System.Security.Cryptography;
using DumDum.Models.Entities;

namespace DumDum.Models.JsonEntities.Battles
{
    public class Target
    {
        public int KingdomId { get; set; }
        public string KingdomName { get; set; }
        public string Ruler { get; set; }
        public Location Location { get; set; }
    }
}