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
            return await UnitOfWork.Players.GetPlayerByUsername(username);
        }

        public async Task<Kingdom> GetKingdomByName(string kingdomName)
        {
            return await UnitOfWork.Kingdoms.GetKingdomByName(kingdomName);
        }

        public async Task<Player> Register(string username, string password, string kingdomName, string email)
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
                    KingdomId = kingdomToSave.Result.KingdomId
                };
                var food = new Resource()
                {
                    Amount = 100, Generation = 1, ResourceType = "Food", UpdatedAt = 1, Kingdom = kingdom,
                    KingdomId = kingdomToSave.Result.KingdomId
                };
                await UnitOfWork.Resources.Add(gold);
                await UnitOfWork.Resources.Add(food);
                UnitOfWork.Complete();
                return kingdomToSave.Result;
            }

            var kingdomTo = UnitOfWork.Kingdoms.AddKingdom(kingdom);
            var golds = new Resource()
            {
                Amount = 100, Generation = 1, ResourceType = "Gold", UpdatedAt = 1, Kingdom = kingdom,
                KingdomId = kingdomTo.Result.KingdomId
            };
            var foods = new Resource()
            {
                Amount = 100, Generation = 1, ResourceType = "Food", UpdatedAt = 1, Kingdom = kingdom,
                KingdomId = kingdomTo.Result.KingdomId
            };
            UnitOfWork.Resources.Add(golds);
            UnitOfWork.Resources.Add(foods);
            UnitOfWork.Complete();
            return kingdomTo.Result;
        }

        public async Task<bool> AreCredentialsValid(string username, string password)
        {
            return await UnitOfWork.Players.AreCredentialsValid(username, password);
        }

        public async Task<bool> AreCoordinatesValid(int coordinateX, int coordinateY)
        {
            return coordinateX > 0 && coordinateX < 100 && coordinateY > 0 && coordinateY < 100;
        }

        public async Task<bool> DoCoordinatesExist(int coordinateX, int coordinateY)
        {
            return UnitOfWork.Kingdoms.Any(k => k.CoordinateX == coordinateX).Result &&
                   UnitOfWork.Kingdoms.Any(k => k.CoordinateX == coordinateX).Result;
        }

        public async Task<bool> IsKingdomIdValid(int kingdomId)
        {
            return UnitOfWork.Players.Any(p => p.KingdomId == kingdomId).Result && UnitOfWork.Kingdoms
                .Any(k => k.KingdomId == kingdomId && k.CoordinateX == 0 && k.CoordinateY == 0).Result;
        }

        public async Task<Kingdom> GetKingdomById(int kingdomId)
        {
            var kingdom = UnitOfWork.Kingdoms.GetKingdomById(kingdomId);
            if (kingdom != null)
            {
                return kingdom.Result;
            }

            return new Kingdom() { };
        }

        public async Task<Kingdom> RegisterKingdomToDB(int coordinateX, int coordinateY, int kingdomId)
        {
            var kingdom = await GetKingdomById(kingdomId);
            kingdom.CoordinateX = coordinateX;
            kingdom.CoordinateY = coordinateY;
            UnitOfWork.Complete();
            return kingdom;
        }

        public async Task<Player> GetPlayerById(int id)
        {
            return await UnitOfWork.Players.GetPlayerById(id);
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

                if (await DoCoordinatesExist(kingdomRequest.CoordinateX, kingdomRequest.CoordinateY))
                {
                    return ("Given coordinates are already taken!", 400);
                }
                if (!await IsKingdomIdValid(kingdomRequest.KingdomId))
                {
                    return ("Kingdom has coordinates assigned already", 400);
                }
                if (await AreCoordinatesValid(kingdomRequest.CoordinateX, kingdomRequest.CoordinateY) &&
                   await IsKingdomIdValid(kingdomRequest.KingdomId) &&
                   !await DoCoordinatesExist(kingdomRequest.CoordinateX, kingdomRequest.CoordinateY) &&
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
                if (await AreCredentialsValid(playerRequest.Username, playerRequest.Password) &&
                    await AuthenticateService.IsEmailValid(playerRequest.Email))
                {
                    var hashedPassword = Crypto.HashPassword(playerRequest.Password);
                    var player = await Register(playerRequest.Username, hashedPassword, playerRequest.KingdomName,
                        playerRequest.Email);
                    await AuthenticateService.SendAccountVerificationEmail(player);
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
                var hashedPassword = Crypto.HashPassword(playerRequest.Password);
                var player = await Register(playerRequest.Username, hashedPassword, playerRequest.KingdomName,
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

        public async Task TakeGold(int kingdomId, int amount)
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

        public async Task TakeFood(int kingdomId, int amount)
        {
            var food = UnitOfWork.Resources.GetFoodAmountOfKingdom(kingdomId).Result;
            if (food != null)
            {
                food.Amount -= amount;
                UnitOfWork.Resources.UpdateFoodAmountOfKingdom(food);
                UnitOfWork.Complete();
            }
        }

        public async Task GiveFood(int kingdomId, int amount)
        {
            var food = UnitOfWork.Resources.GetFoodAmountOfKingdom(kingdomId).Result;
            if (food != null)
            {
                food.Amount += amount;
                UnitOfWork.Resources.UpdateGoldAmountOfKingdom(food);
                UnitOfWork.Complete();
            }
        }

        public async Task GiveGold(int kingdomId, int amount)
        {
            var gold = UnitOfWork.Resources.GetFoodAmountOfKingdom(kingdomId).Result;
            if (gold != null)
            {
                gold.Amount += amount;
                UnitOfWork.Resources.UpdateGoldAmountOfKingdom(gold);
                UnitOfWork.Complete();
            }
        }

        public async Task<(string, int)> SetAuthToTrue(int playerId, string hash)
        {
            var player = await GetPlayerVerified(playerId, hash);
            if (player is not null)
            {
                if (player.IsVerified)
                {
                    return ("Email already verified!", 200);
                }
                player.IsVerified = true;
                UnitOfWork.Players.Update(player);
                UnitOfWork.Complete();
                return ($"{player.Email} is now a verified email!", 200);
            }
            return (string.Empty, 400);
        }

        public async Task<(string, int)> ResetPassword(PasswordResetRequest passwordResetRequest)
        {
            if (UnitOfWork.Players.UserWithEmailExists(passwordResetRequest.Username, passwordResetRequest.Email))
            {
                var player = await GetPlayerByUsername(passwordResetRequest.Username);
                AuthenticateService.SendPasswordResetEmail(player);
                return ("Email has been sent.", 200);
            }
            return ("Credentials not valid.", 400);
        }

        public async Task<Player> GetPlayerVerified(int playerId, string hash)
        {
            hash = hash.Replace(" ", "+");
            var player = await GetPlayerById(playerId);
            if (player is not null && player.Password.Contains(hash))
            {
                return player;
            }

            return null;
        }

        public async Task<(string, int)> ChangePassword(int playerId, string newPassword)
        {
            if (newPassword is not null && playerId != 0)
            {
                var player = await GetPlayerById(playerId);
                var hashed = Crypto.HashPassword(newPassword);
                if (newPassword.Length >= 8 && hashed != player.Password)
                {
                    player.Password = Crypto.HashPassword(newPassword);
                    UnitOfWork.Players.Update(player);
                    UnitOfWork.Complete();
                    return ("Password has been changed successfully.", 200);
                }
                return ("password doesn't match required conditions", 400);
            }
            return ("Error. Something went wrong with the request.", 400);
        }
    }
}