using DumDum.Models.Entities;

namespace DumDum.Models.JsonEntities
{
    public class PlayerJson
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string KingdomName { get; set; }
        public int KingdomId { get; set; }
    }
}