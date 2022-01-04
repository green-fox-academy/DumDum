namespace DumDum.Models.JsonEntities.Kingdom
{
    public class KingdomRenameResponse
    {
        public int KingdomId { get; set; }
        public string KingdomName { get; set; }
        
        public KingdomRenameResponse(Entities.Player player, KingdomRenameRequest kingdomRenameRequest)
        {
            KingdomId = player.KingdomId; 
            KingdomName = player.Kingdom.KingdomName;
            player.Kingdom.KingdomName = kingdomRenameRequest.KingdomName;
        }
    }
}