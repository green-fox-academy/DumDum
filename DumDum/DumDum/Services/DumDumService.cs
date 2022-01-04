using System.Linq;
using System.Web.Helpers;
using Castle.Core.Internal;
using DumDum.Database;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Kingdom;
using DumDum.Models.JsonEntities.Player;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;


namespace DumDum.Services
{
    public class DumDumService
    {
        private ApplicationDbContext DbContext { get; set; }
        private AuthenticateService AuthenticateService { get; set; }
        public DumDumService(ApplicationDbContext dbContext, AuthenticateService authService)
        {
            DbContext = dbContext;
            AuthenticateService = authService;
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
            var player = new Player() { Password = password, Username = username, KingdomId = kingdom.KingdomId };
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
                kingdom.KingdomName = $"{username}'s Kingdom";
                var kingdomToSave = DbContext.Kingdoms.Add(kingdom).Entity;
                var gold = new Resource()
                {
                    Amount = 100, Generation = 1, ResourceType = "Gold", UpdatedAt = 1, Kingdom = kingdom,
                    KingdomId = kingdomToSave.KingdomId
                };
                var food = new Resource()
                {
                    Amount = 100, Generation = 1, ResourceType = "Food", UpdatedAt = 1, Kingdom = kingdom,
                    KingdomId = kingdomToSave.KingdomId
                };
                DbContext.Resources.Add(gold);
                DbContext.Resources.Add(food);
                DbContext.SaveChanges();
                return kingdomToSave;
            }

            kingdom.KingdomName = kingdomname;
            var kingdomTo = DbContext.Kingdoms.Add(kingdom).Entity;
            var golds = new Resource()
            {
                Amount = 100, Generation = 1, ResourceType = "Gold", UpdatedAt = 1, Kingdom = kingdom,
                KingdomId = kingdomTo.KingdomId
            };
            var foods = new Resource()
            {
                Amount = 100, Generation = 1, ResourceType = "Food", UpdatedAt = 1, Kingdom = kingdom,
                KingdomId = kingdomTo.KingdomId
            };
            DbContext.Resources.Add(golds);
            DbContext.Resources.Add(foods);
            DbContext.SaveChanges();
            return kingdomTo;
        }

        public bool AreCredentialsValid(string username, string password)
        {
            return DbContext.Players.Any(p => p.Username != username) &&
                   !string.IsNullOrWhiteSpace(username) && password.Length >= 8;
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
            return DbContext.Players.Any(p => p.KingdomId == kingdomId) && DbContext.Kingdoms.Any(k =>
                k.KingdomId == kingdomId && k.CoordinateX == 0 && k.CoordinateY == 0);
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

        public Kingdom RegisterKingdomToDB(int coordinateX, int coordinateY, int kingdomId)
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

        public string RegisterKingdom(string authorization, KingdomRegistrationRequest kingdomRequest, out int statusCode)
        {
            if (authorization != "")
            {
                var player = AuthenticateService.GetUserInfo(new AuthRequest() { Token = authorization });

                if (kingdomRequest == null || kingdomRequest.GetType().GetProperties().All(p=>p.GetValue(kingdomRequest) == null))
                {
                    statusCode = 400;
                    return "Request was not done correctly!";
                }

                if (player.KingdomId != kingdomRequest.KingdomId)
                {
                    statusCode = 401;
                    return "This kingdom does not belong to authenticated player";
                }

                if (!AreCoordinatesValid(kingdomRequest.CoordinateX, kingdomRequest.CoordinateY))
                {
                    statusCode = 400;
                    return "One or both coordinates are out of valid range(0 - 99).";
                }

                if (DoCoordinatesExist(kingdomRequest.CoordinateX, kingdomRequest.CoordinateY))
                {
                    statusCode = 400;
                    return "Given coordinates are already taken!";
                }

                if (!IsKingdomIdValid(kingdomRequest.KingdomId))
                {
                    statusCode = 400;
                    return "Kingdom has coordinates assigned already";
                }

                if (AreCoordinatesValid(kingdomRequest.CoordinateX, kingdomRequest.CoordinateY) &&
                    IsKingdomIdValid(kingdomRequest.KingdomId) &&
                    !DoCoordinatesExist(kingdomRequest.CoordinateX, kingdomRequest.CoordinateY) &&
                    player != null && player.KingdomId == kingdomRequest.KingdomId)
                {
                    RegisterKingdomToDB(kingdomRequest.CoordinateX, kingdomRequest.CoordinateY, kingdomRequest.KingdomId);
                    statusCode = 200;
                    return "Ok";
                }
            }

            statusCode = 400;
            return "";
        }

        public PlayerResponse RegisterPlayerLogic(PlayerRequest playerRequest, out int statusCode)
        {
            if (playerRequest.KingdomName is not null)
            {
                if (AreCredentialsValid(playerRequest.Username, playerRequest.Password))
                {
                    var hashedPassword = Crypto.HashPassword(playerRequest.Password);
                    var player = Register(playerRequest.Username, hashedPassword, playerRequest.KingdomName);
                    if (player is null)
                    {
                        statusCode = 400;
                        return null;
                    }

                    statusCode = 200;
                    return new PlayerResponse() {Username = player.Username, KingdomId = player.KingdomId};
                }

                statusCode = 400;
                return null;
            }

            if (AreCredentialsValid(playerRequest.Username, playerRequest.Password))
            {
                var player = Register(playerRequest.Username, playerRequest.Password, playerRequest.KingdomName);
                if (player is null)
                {
                    statusCode = 400;
                    return null;
                }

                statusCode = 200;
                return new PlayerResponse() {Username = player.Username, KingdomId = player.KingdomId};
            }

            statusCode = 400;
            return null;
        }
        
        public KingdomsListResponse GetAllKingdoms()
        {

            KingdomsListResponse response = new KingdomsListResponse();

            response.Kingdoms = DbContext.Kingdoms.Include(k => k.Player).Select(k => new KingdomResponse()
            {
                KingdomId = k.KingdomId,
                KingdomName = k.KingdomName,
                Ruler = k.Player.Username,
                Population = 0,
                Location = new Location()
                {
                    CoordinateX = k.CoordinateX,
                    CoordinateY = k.CoordinateY,
                }
            }).ToList();

            return response;

        }

        public Location AddLocations(Kingdom kingdom)
        {
            return new Location() {CoordinateX = kingdom.CoordinateX, CoordinateY = kingdom.CoordinateY};
        }

        public int GetGoldAmountOfKingdom(int kingdomId)
        {
            if (kingdomId != 0)
            {
                var gold = DbContext.Resources.FirstOrDefault(r =>
                    r.KingdomId == kingdomId && r.ResourceType == "Gold");
                if (gold != null)
                {
                    return gold.Amount;
                }

                return 0;
            }

            return 0;
        }

        public int GetFoodAmountOfKingdom(int kingdomId)
        {
            if (kingdomId != 0)
            {
                var food = DbContext.Resources.FirstOrDefault(r =>
                    r.KingdomId == kingdomId && r.ResourceType == "Food");
                if (food != null)
                {
                    return food.Amount;
                }

                return 0;
            }

            return 0;
        }

        public void TakeGold(int kingdomId, int amount)
        {
            var gold = DbContext.Resources.FirstOrDefault(r => r.KingdomId == kingdomId && r.ResourceType == "Gold");
            if (gold != null)
            {
                gold.Amount -= amount;
                DbContext.Resources.Update(gold);
                DbContext.SaveChanges();
            }
        }      
        

        public void TakeFood(int kingdomId, int amount)
        {
            var food = DbContext.Resources.FirstOrDefault(r => r.KingdomId == kingdomId && r.ResourceType == "Food");
            if (food != null)
            {
                food.Amount -= amount;
                DbContext.Resources.Update(food);
                DbContext.SaveChanges();
            }
        }

        public void GiveFood(int kingdomId, int amount)
        {
            var food = DbContext.Resources.FirstOrDefault(r => r.KingdomId == kingdomId && r.ResourceType == "Food");
            if (food != null)
            {
                food.Amount += amount;
                DbContext.Resources.Update(food);
                DbContext.SaveChanges();
            }
        }

        public void GiveGold(int kingdomId, int amount)
        {
            var gold = DbContext.Resources.FirstOrDefault(r => r.KingdomId == kingdomId && r.ResourceType == "Gold");
            if (gold != null)
            {
                gold.Amount += amount;
                DbContext.Resources.Update(gold);
                DbContext.SaveChanges();
            }
        }
    }
}