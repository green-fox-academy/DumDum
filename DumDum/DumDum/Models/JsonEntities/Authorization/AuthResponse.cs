namespace DumDum.Models.JsonEntities.Authorization
{
    public class AuthResponse
    {
        public string Ruler { get; set; }
        public int KingdomId { get; set; }
        public string KingdomName { get; set; }

        public AuthResponse()
        {
                
        }
        public AuthResponse(Entities.Player player)
        {
            Ruler = player.Username;
            KingdomId = player.KingdomId;
            KingdomName = player.Kingdom.KingdomName;
        }

    }
}