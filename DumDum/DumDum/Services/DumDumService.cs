using Castle.Core.Internal;
using DumDum.Interfaces;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Kingdom;
using DumDum.Models.JsonEntities.Player;
using System.Linq;
using System.Web.Helpers;


namespace DumDum.Services
{
    public class DumDumService : IDumDumService
    {
        private IAuthenticateService AuthenticateService { get; set; }
        private IUnitOfWork UnitOfWork { get; set; }
        public DumDumService(IAuthenticateService authService, IUnitOfWork unitOfWork)
        {
            AuthenticateService = authService;
            UnitOfWork = unitOfWork;
        }

        public Player GetPlayerByUsername(string username)
        {
            return UnitOfWork.Players.GetPlayerByUsername(username);
        }

        public Kingdom GetKingdomByName(string kingdomName)
        {
            return UnitOfWork.Kingdoms.GetKingdomByName(kingdomName);
        }

        public Player Register(string username, string password, string kingdomName)
        {
            var kingdom = CreateKingdom(kingdomName, username);
            var player = new Player() {Password = password, Username = username, KingdomId = kingdom.KingdomId};
            UnitOfWork.Players.Add(player);
            UnitOfWork.Complete();
            var playerToReturn = GetPlayerByUsername(username);
            kingdom.PlayerId = playerToReturn.PlayerId;
            UnitOfWork.Complete();
            return playerToReturn;
        }

        public Kingdom CreateKingdom(string kingdomname, string username)
        {
            var kingdom = new Kingdom();
            if (kingdomname.IsNullOrEmpty())
            {
                kingdom.KingdomName = $"{username}'s Kingdom";
                var kingdomToSave = UnitOfWork.Kingdoms.AddKingdom(kingdom);
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
                UnitOfWork.Resources.Add(gold);
                UnitOfWork.Resources.Add(food);
                UnitOfWork.Complete();
                return kingdomToSave;
            }

            var kingdomTo = UnitOfWork.Kingdoms.AddKingdom(kingdom);
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
            UnitOfWork.Resources.Add(golds);
            UnitOfWork.Resources.Add(foods);
            UnitOfWork.Complete();
            return kingdomTo;
        }

        public bool AreCredentialsValid(string username, string password)
        {
            return UnitOfWork.Players.AreCredentialsValid(username, password);
        }

        public bool AreCoordinatesValid(int coordinateX, int coordinateY)
        {
            return coordinateX > 0 && coordinateX < 100 && coordinateY > 0 && coordinateY < 100;
        }

        public bool DoCoordinatesExist(int coordinateX, int coordinateY)
        {
            return UnitOfWork.Kingdoms.Any(k => k.CoordinateX == coordinateX) &&
                   UnitOfWork.Kingdoms.Any(k => k.CoordinateX == coordinateX);
        }

        public bool IsKingdomIdValid(int kingdomId)
        {
            return UnitOfWork.Players.Any(p => p.KingdomId == kingdomId) && UnitOfWork.Kingdoms
                .Any(k => k.KingdomId == kingdomId && k.CoordinateX == 0 && k.CoordinateY == 0);
        }

        public Kingdom GetKingdomById(int kingdomId)
        {
            var kingdom = UnitOfWork.Kingdoms.GetKingdomById(kingdomId);
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
            UnitOfWork.Complete();
            return kingdom;
        }

        public Player GetPlayerById(int id)
        {
            return UnitOfWork.Players.GetPlayerById(id);
        }

        public string RegisterKingdom(string authorization, KingdomRegistrationRequest kingdomRequest,
            out int statusCode)
        {
            if (authorization != "")
            {
                var player = AuthenticateService.GetUserInfo(new AuthRequest() {Token = authorization});

                if (kingdomRequest == null || kingdomRequest.GetType().GetProperties()
                    .All(p => p.GetValue(kingdomRequest) == null))
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
                    RegisterKingdomToDB(kingdomRequest.CoordinateX, kingdomRequest.CoordinateY,
                        kingdomRequest.KingdomId);
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
            return UnitOfWork.Kingdoms.GetAllKingdoms();
        }

        public Location AddLocations(Kingdom kingdom)
        {
            return new Location() {CoordinateX = kingdom.CoordinateX, CoordinateY = kingdom.CoordinateY};
        }

        public int GetGoldAmountOfKingdom(int kingdomId)
        {
            if (kingdomId != 0)
            {
                var gold = UnitOfWork.Resources.GetGoldAmountOfKingdom(kingdomId);
                if (gold != null)
                {
                    return gold.Amount;
                }

                return 0;
            }

            return 0;
        }

        public void TakeGold(int kingdomId, int amount)
        {
            var gold = UnitOfWork.Resources.GetGoldAmountOfKingdom(kingdomId);
            if (gold != null)
            {
                gold.Amount -= amount;
                UnitOfWork.Resources.UpdateGoldAmountOfKingdom(gold);
                UnitOfWork.Complete();
            }
        }

        public int GetFoodAmountOfKingdom(int kingdomId)
        {
            if (kingdomId != 0)
            {
                var food = UnitOfWork.Resources.GetFoodAmountOfKingdom(kingdomId);
                if (food != null)
                {
                    return food.Amount;
                }
                return 0;
            }
            return 0;
        }

        public void TakeFood(int kingdomId, int amount)
        {
            var food = UnitOfWork.Resources.GetFoodAmountOfKingdom(kingdomId);
            if (food != null)
            {
                food.Amount -= amount;
                UnitOfWork.Resources.UpdateFoodAmountOfKingdom(food);
                UnitOfWork.Complete();
            }
        }

        public void GiveFood(int kingdomId, int amount)
        {
            var food = UnitOfWork.Resources.GetFoodAmountOfKingdom(kingdomId);
            if (food != null)
            {
                food.Amount += amount;
                UnitOfWork.Resources.UpdateGoldAmountOfKingdom(food);
                UnitOfWork.Complete();
            }
        }

        public void GiveGold(int kingdomId, int amount)
        {
            var gold = UnitOfWork.Resources.GetFoodAmountOfKingdom(kingdomId);
            if (gold != null)
            {
                gold.Amount += amount;
                UnitOfWork.Resources.UpdateGoldAmountOfKingdom(gold);
                UnitOfWork.Complete();
            }
        }
    }
}