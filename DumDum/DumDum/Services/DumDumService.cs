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
            var player = GetPlayerByUsername(username);
            var kingdom = new Kingdom() {KingdomName = $"{username}'s kingdom", PlayerId = player.PlayerId};
            DbContext.Kingdoms.Add(kingdom);
            DbContext.SaveChanges();
            var kingdomToReturn = GetKingdomByName(kingdom.KingdomName);
            return kingdomToReturn;
        }

        public bool IsValid(string username, string password)
        {
            if (username != string.Empty && DbContext.Players.Any(p => p.Username != username))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        internal bool AreCoordinatesValid(int coordinateX, int coordinateY)
        {
            if (coordinateX > 0 && coordinateX < 100 && coordinateY > 0 && coordinateY < 100)
            {
                return true;
            }
            return false;
        }

        internal bool DoCoordinatesExist(int coordinateX, int coordinateY)
        {
            if (DbContext.Kingdoms.Any(k => k.CoordinateX == coordinateX) || DbContext.Kingdoms.Any(k => k.CoordinateY == coordinateY))
            {
                return true;
            }
            return false;
        }

        internal bool IsKingdomIdValid(int kingdomId)
        {
            if (DbContext.Players.Any(p => p.KingdomId == kingdomId))
            {
                return true;
            }
            return false;
        }

        public Kingdom GetKingdomById(int kingdomId)
        {
            return DbContext.Kingdoms.FirstOrDefault(x => x.KingdomId == kingdomId);
        }

        public Kingdom RegisterKingdom(int coordinateX, int coordinateY, int kingdomId)
        {
            var kingdom = GetKingdomById(kingdomId);
            kingdom.CoordinateX = coordinateX;
            kingdom.CoordinateY = coordinateY;
            DbContext.SaveChanges();
            return kingdom;
        }
    }
}