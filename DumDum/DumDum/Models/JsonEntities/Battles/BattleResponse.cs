namespace DumDum.Models.JsonEntities.Battles
{
    public class BattleResponse
    {
        public int BattleId { get; set; }
        public long ResolutionTime { get; set; }

        public BattleResponse()
        {
            
        }

        public BattleResponse(int battleId, long resolutionTime )
        {
            BattleId = battleId;
            ResolutionTime = resolutionTime;
        }
    }
}