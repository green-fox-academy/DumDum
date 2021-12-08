using System.Linq;
using DumDum.Database;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;

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

        public Player Register(string username, string password, Kingdom kingdom)
        {
            var player = new Player() {Password = password, Username = username, KingdomId = kingdom.KingdomId};
            kingdom.PlayerId = player.PlayerId;
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

        public bool IsValid(string username, string password)
        {
            if (!string.IsNullOrEmpty(username) && DbContext.Players.Any(p => p.Username != username) && !string.IsNullOrWhiteSpace(username))
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
            return (coordinateX > 0 && coordinateX < 100 && coordinateY > 0 && coordinateY < 100);
        }

        internal bool DoCoordinatesExist(int coordinateX, int coordinateY)
        {
            return (DbContext.Kingdoms.Any(k => k.CoordinateX == coordinateX) || DbContext.Kingdoms.Any(k => k.CoordinateY == coordinateY));
        }

        internal bool IsKingdomIdValid(int kingdomId)
        {
            return DbContext.Players.Any(p => p.KingdomId == kingdomId);
        }

        public Kingdom GetKingdomById(int kingdomId)
        {
            var kingdom= DbContext.Kingdoms.FirstOrDefault(x => x.KingdomId == kingdomId);
            if (kingdom != null)
            {
                return kingdom;
            }
            return new Kingdom() { };
        }

        public Kingdom RegisterKingdom(int coordinateX, int coordinateY, int kingdomId)
        {
            var kingdom = GetKingdomById(kingdomId);
            kingdom.CoordinateX = coordinateX;
            kingdom.CoordinateY = coordinateY;
            DbContext.SaveChanges();
            return kingdom;
        }

        public string RegisterKingdomLogic(KingdomJson kingdomJson, out int statusCode)
        {
            if (AreCoordinatesValid(kingdomJson.CoordinateX, kingdomJson.CoordinateY) && IsKingdomIdValid(kingdomJson.KingdomId) &&
                !DoCoordinatesExist(kingdomJson.CoordinateX, kingdomJson.CoordinateY))
            {
                RegisterKingdom(kingdomJson.CoordinateX, kingdomJson.CoordinateY, kingdomJson.KingdomId);
                statusCode = 200;
                return "Ok";
            }
            if (!AreCoordinatesValid(kingdomJson.CoordinateX, kingdomJson.CoordinateY))
            {
                statusCode = 400;
                return "One or both coordinates are out of valid range(0 - 99).";
            }
            if (DoCoordinatesExist(kingdomJson.CoordinateX, kingdomJson.CoordinateY))
            {
                statusCode = 400;
                return "Given coordinates are already taken!";
            }
            statusCode = 400;
            return "";
        }
    }
}