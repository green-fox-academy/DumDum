using System.Linq;
using Castle.Core.Internal;
using DumDum.Database;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using Microsoft.EntityFrameworkCore;

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
            return DbContext.Players.Include(p => p.Kingdom).FirstOrDefault(p => p.Username == username);
        }

        public Kingdom GetKingdomByName(string kingdomName)
        {
            return DbContext.Kingdoms.Include(k => k.Player).FirstOrDefault(x => x.KingdomName == kingdomName);
        }

        public Player Register(string username, string password, string kingdomName)
        {
            var kingdom = CreateKingdom(kingdomName, username);
            var player = new Player() {Password = password, Username = username, KingdomId = kingdom.KingdomId};
            DbContext.Players.Add(player);
            DbContext.SaveChanges();
            var playerToReturn = GetPlayerByUsername(username);
            kingdom.PlayerId = playerToReturn.PlayerId;
            DbContext.SaveChanges();
            return playerToReturn;
        }

        public Kingdom CreateKingdom(string kingdomname, string username)
        {
            var kingdom = new Kingdom();
            if (kingdomname.IsNullOrEmpty())
            {
                kingdom.KingdomName = $"{username}'s kingdom";
                DbContext.Kingdoms.Add(kingdom);
            }

            kingdom.KingdomName = kingdomname;
            DbContext.Kingdoms.Add(kingdom);
            DbContext.SaveChanges();
            return kingdom;
        }

        public bool IsValid(string username, string password)
        {
            if (!string.IsNullOrEmpty(username) && DbContext.Players.Any(p => p.Username != username) &&
                !string.IsNullOrWhiteSpace(username))
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
            return coordinateX > 0 && coordinateX < 100 && coordinateY > 0 && coordinateY < 100;
        }

        internal bool DoCoordinatesExist(int coordinateX, int coordinateY)
        {
            return DbContext.Kingdoms.Any(k => k.CoordinateX == coordinateX) ||
                   DbContext.Kingdoms.Any(k => k.CoordinateY == coordinateY);
        }

        internal bool IsKingdomIdValid(int kingdomId)
        {
            return DbContext.Players.Any(p => p.KingdomId == kingdomId);
        }

        public Kingdom GetKingdomById(int kingdomId)
        {
            var kingdom = DbContext.Kingdoms.Include(k => k.Player).FirstOrDefault(x => x.KingdomId == kingdomId);
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

        public Player GetPlayerById(int id)
        {
            return DbContext.Players.Include(p => p.Kingdom).FirstOrDefault(p => p.PlayerId == id);
        }

        public string RegisterKingdomLogic(KingdomJson kingdomJson, out int statusCode)
        {
            if (AreCoordinatesValid(kingdomJson.CoordinateX, kingdomJson.CoordinateY) &&
                IsKingdomIdValid(kingdomJson.KingdomId) &&
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

        public PlayerResponse RegisterPlayerLogic(PlayerRequest playerRequest, out int statusCode)
        {
            if (playerRequest.KingdomName is not null)
            {
                var player = Register(playerRequest.Username, playerRequest.Password, playerRequest.KingdomName);
                if (IsValid(playerRequest.Username, playerRequest.Password))
                {
                    var kingdom = GetKingdomByName(playerRequest.KingdomName);
                    statusCode = 200;
                    return new PlayerResponse() {Username = player.Username, KingdomId = player.KingdomId};
                }

                statusCode = 400;
                return null;
            }

            if (IsValid(playerRequest.Username, playerRequest.Password))
            {
                var player = Register(playerRequest.Username, playerRequest.Password, playerRequest.KingdomName);
                var newKingdom = GetKingdomByName(playerRequest.KingdomName);
                statusCode = 200;
                return new PlayerResponse() {Username = player.Username, KingdomId = player.KingdomId};
            }

            statusCode = 400;
            return null;
        }

        public Kingdom KingdomInformation(int kingdomId)
        {
            var kingdom = DbContext.Kingdoms.Include(r => r.Resources).FirstOrDefault(k => k.KingdomId == kingdomId);
            return kingdom;
        }
    }
}