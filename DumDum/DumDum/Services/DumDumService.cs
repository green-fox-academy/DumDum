using System.Linq;
using DumDum.Database;
using DumDum.Models.Entities;

namespace DumDum.Services
{
    public class DumDumService
    {
        private ApplicationDbContext DbContext { get; set; }

        public DumDumService(ApplicationDbContext dbContex)
        {
            DbContext = dbContex;
        }

        public Player GetPlayerByUsername(string username)
        {
            return DbContext.Players.FirstOrDefault(p => p.Username == username);
        }

    public Kingdom GetKingdomByName(string kingdomName)
        {
            return DbContext.Kingdoms.FirstOrDefault(x => x.KingdomName == kingdomName);
        }

        public Player Register(string username, string password, int kingdomId)
        {
            var player = new Player() {Password = password, Username = username, KingdomId = kingdomId};
            DbContext.Players.Add(player);
            DbContext.SaveChanges();
            var playerToReturn = GetPlayerByUsername(username);
            return playerToReturn;
        }

        public Kingdom CreateKingdom(string username)
        {
            var kingdom = new Kingdom() {KingdomName = $"{username}'s kingdom"};
            DbContext.Kingdoms.Add(kingdom);
            DbContext.SaveChanges();
            var kingdomToReturn = GetKingdomByName(kingdom.KingdomName);
            return kingdomToReturn;
        }
    }
}