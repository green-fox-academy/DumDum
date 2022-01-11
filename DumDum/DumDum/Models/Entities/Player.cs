namespace DumDum.Models.Entities
{
    public class Player
    {
        public int PlayerId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool IsVerified { get; set; }
        public Kingdom Kingdom { get; set; }
        public int KingdomId { get; set; }
        public LastChange LastChange { get; set; }
    }
}