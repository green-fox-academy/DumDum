namespace DumDum.Models.Entities
{
    public class Player
    {
        public int PlayerId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Kingdom Kingdom { get; set; }
        public int KingdomId { get; set; }
        public string Token { get; set; }
    }
}