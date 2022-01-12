using Castle.Core.Internal;
using DumDum.Interfaces;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Kingdom;
using DumDum.Models.JsonEntities.Player;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<Player> GetPlayerByUsername(string username)
        {
            return UnitOfWork.Players.GetPlayerByUsername(username).Result;
        }

        public Task<Kingdom> GetKingdomByName(string kingdomName)
        {
            return UnitOfWork.Kingdoms.GetKingdomByName(kingdomName);
        }

        public async Task<Player> Register(string username, string password, string kingdomName)
        {
            var kingdom = await CreateKingdom(kingdomName, username);
            var player = new Player() { Password = password, Username = username, KingdomId = kingdom.KingdomId };
            UnitOfWork.Players.Add(player);
            UnitOfWork.Complete();
            var playerToReturn = await GetPlayerByUsername(username);
            kingdom.PlayerId = playerToReturn.PlayerId;
            UnitOfWork.Complete();
            return playerToReturn;
        }

        public async Task<Kingdom> CreateKingdom(string kingdomname, string username)
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

        public async Task<bool> AreCredentialsValid(string username, string password)
        {
            return UnitOfWork.Players.AreCredentialsValid(username, password).Result;
        }

        internal async Task<bool> AreCoordinatesValid(int coordinateX, int coordinateY)
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
                return kingdom.Result;
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
            return UnitOfWork.Players.GetPlayerById(id).Result;
        }

        public async Task<(string, int)> RegisterKingdom(string authorization, KingdomRegistrationRequest kingdomRequest)
        {
            if (authorization != "")
            {
                var player = await AuthenticateService.GetUserInfo(new AuthRequest() { Token = authorization });

                if (kingdomRequest == null || kingdomRequest.GetType().GetProperties()
                    .All(p => p.GetValue(kingdomRequest) == null))
                {
                    return ("Request was not done correctly!", 400);
                }
                if (player.KingdomId != kingdomRequest.KingdomId)
                {
                    return ("This kingdom does not belong to authenticated player", 401);
                }
                if (!await AreCoordinatesValid(kingdomRequest.CoordinateX, kingdomRequest.CoordinateY))
                {
                    return ("One or both coordinates are out of valid range(0 - 99).", 400);
                }

                if (DoCoordinatesExist(kingdomRequest.CoordinateX, kingdomRequest.CoordinateY))
                {
                    return ("Given coordinates are already taken!", 400);
                }
                if (!IsKingdomIdValid(kingdomRequest.KingdomId))
                {
                    return ("Kingdom has coordinates assigned already", 400);
                }
                if (await AreCoordinatesValid(kingdomRequest.CoordinateX, kingdomRequest.CoordinateY) &&
                   IsKingdomIdValid(kingdomRequest.KingdomId) &&
                   !DoCoordinatesExist(kingdomRequest.CoordinateX, kingdomRequest.CoordinateY) &&
                   player != null && player.KingdomId == kingdomRequest.KingdomId)
                {
                    RegisterKingdomToDB(kingdomRequest.CoordinateX, kingdomRequest.CoordinateY, kingdomRequest.KingdomId);
                    return ("Ok", 200);
                }
            }
            return ("",400);
        }

        public async Task<(PlayerResponse, int)> RegisterPlayerLogic(PlayerRequest playerRequest)
        {
            if (playerRequest.KingdomName is not null && playerRequest.Email is not null)
            {
                if (AreCredentialsValid(playerRequest.Username, playerRequest.Password) &&
                    AuthenticateService.IsEmailValid(playerRequest.Email))
                {
                    var hashedPassword = Crypto.HashPassword(playerRequest.Password);
                    var player = Register(playerRequest.Username, hashedPassword, playerRequest.KingdomName,
                        playerRequest.Email);
                    AuthenticateService.SendAccountVerificationEmail(player);
                    if (player is null)
                    {
                        return (null, 400);
                    }
                    var response = new PlayerResponse() { Username = player.Username, KingdomId = player.KingdomId };
                    return (response, 200);
                }
                return (null, 400);
            }

            if (await AreCredentialsValid(playerRequest.Username, playerRequest.Password))
            {
                var player = Register(playerRequest.Username, playerRequest.Password, playerRequest.KingdomName,
                    playerRequest.Email);
                if (player is null)
                {
                    return (null, 400);
                }
                var response = new PlayerResponse() { Username = player.Username, KingdomId = player.KingdomId };
                return (response, 200);
            }
            return (null, 400);
        }

        public async Task<KingdomsListResponse> GetAllKingdoms()
        {
            return UnitOfWork.Kingdoms.GetAllKingdoms().Result;
        }

        public async Task<Location> AddLocations(Kingdom kingdom)
        {
            return new Location() {CoordinateX = kingdom.CoordinateX, CoordinateY = kingdom.CoordinateY};
        }
        
        public async Task<int> GetGoldAmountOfKingdom(int kingdomId)
        {
            if (kingdomId != 0)
            {
                var gold = await UnitOfWork.Resources.GetGoldAmountOfKingdom(kingdomId);
                if (gold != null)
                {
                    int amount = gold.Amount;
                    return amount;
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
                gold.Result.Amount -= amount;
                UnitOfWork.Resources.UpdateGoldAmountOfKingdom(gold.Result);
                UnitOfWork.Complete();
            }
        }

        public async Task<int> GetFoodAmountOfKingdom(int kingdomId)
        {
            if (kingdomId != 0)
            {
                var food = await UnitOfWork.Resources.GetFoodAmountOfKingdom(kingdomId);
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
            var food = UnitOfWork.Resources.GetFoodAmountOfKingdom(kingdomId).Result;
            if (food != null)
            {
                food.Amount -= amount;
                UnitOfWork.Resources.UpdateFoodAmountOfKingdom(food);
                UnitOfWork.Complete();
            }
        }

        public void GiveFood(int kingdomId, int amount)
        {
            var food = UnitOfWork.Resources.GetFoodAmountOfKingdom(kingdomId).Result;
            if (food != null)
            {
                food.Amount += amount;
                UnitOfWork.Resources.UpdateGoldAmountOfKingdom(food);
                UnitOfWork.Complete();
            }
        }

        public void GiveGold(int kingdomId, int amount)
        {
            var gold = UnitOfWork.Resources.GetFoodAmountOfKingdom(kingdomId).Result;
            if (gold != null)
            {
                gold.Amount += amount;
                UnitOfWork.Resources.UpdateGoldAmountOfKingdom(gold);
                UnitOfWork.Complete();
            }
        }

        public string SetAuthToTrue(int playerId, string hash, out int statusCode)
        {
            var player = GetPlayerVerified(playerId, hash);
            if (player is not null)
            {
                if (player.IsVerified)
                {
                    statusCode = 200;
                    return "Email already verified!";
                }
                statusCode = 200;
                player.IsVerified = true;
                UnitOfWork.Players.Update(player);
                UnitOfWork.Complete();
                return $"{player.Email} is now a verified email!";
            }

            statusCode = 400;
            return string.Empty;
        }

        public string ResetPassword(PasswordResetRequest passwordResetRequest, out int statusCode)
        {
            if (UnitOfWork.Players.UserWithEmailExists(passwordResetRequest.Username, passwordResetRequest.Email))
            {
                var player = GetPlayerByUsername(passwordResetRequest.Username);
                AuthenticateService.SendPasswordResetEmail(player);
                statusCode = 200;
                return "Email has been sent.";
            }

            statusCode = 400;
            return "Credentials not valid.";
        }

        public Player GetPlayerVerified(int playerId, string hash)
        {
            hash = hash.Replace(" ", "+");
            var player = GetPlayerById(playerId);
            if (player is not null && player.Password.Contains(hash))
            {
                return player;
            }

            return null;
        }

        public string ChangePassword(int playerId, string newPassword, out int statusCode)
        {
            if (newPassword is not null && playerId != 0)
            {
                var player = GetPlayerById(playerId);
                var hashed = Crypto.HashPassword(newPassword);
                if (newPassword.Length >= 8 && hashed != player.Password)
                {
                    player.Password = Crypto.HashPassword(newPassword);
                    UnitOfWork.Players.Update(player);
                    UnitOfWork.Complete();
                    statusCode = 200;
                    return "Password has been changed successfully.";
                }

                statusCode = 400;
                return "password doesn't match required conditions";
            }

            statusCode = 400;
            return "Error. Something went wrong with the request.";
        }
    }
}