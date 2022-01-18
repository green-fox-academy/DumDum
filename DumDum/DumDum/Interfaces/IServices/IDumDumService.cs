using System.Threading.Tasks;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Kingdom;
using DumDum.Models.JsonEntities.Player;

namespace DumDum.Interfaces.IServices
{
    public interface IDumDumService
    {
        Task<(PlayerResponse, int)> RegisterPlayerLogic(PlayerRequest playerRequest);
        Task<(string, int)> RegisterKingdom(string authorization, KingdomRegistrationRequest kingdomRequest);
        Task<KingdomsListResponse> GetAllKingdoms();
        Task<Player> GetPlayerByUsername(string username);
        Task<Kingdom> GetKingdomById(int kingdomId);
        Task<int> GetGoldAmountOfKingdom(int kingdomId);
        Task TakeGold(int kingdomId, int amount);
        Task TakeFood(int kingdomId, int amount);
        Task<Player> GetPlayerById(int id);
        Task<Location> AddLocations(Kingdom kingdom);
        Task<int> GetFoodAmountOfKingdom(int kingdomId);
        Task GiveGold(int kingdomId, int amount);
        Task GiveFood(int kingdomId, int amount);
        Task<Kingdom> RegisterKingdomToDb(int coordinateX, int coordinateY, int kingdomId);
        Task<bool> IsKingdomIdValid(int kingdomId);
        Task<bool> AreCredentialsValid(string username, string password);
        Task<bool> AreCoordinatesValid(int coordinateX, int coordinateY);
        Task<bool> DoCoordinatesExist(int coordinateX, int coordinateY);
        Task<Kingdom> CreateKingdom(string kingdomName, string username);
        Task<Player> Register(string username, string password, string kingdomName, string email);
        Task<Kingdom> GetKingdomByName(string kingdomName);
        Task<(string, int)> SetAuthToTrue(int playerId, string hash);
        Task<(string, int)> ResetPassword(PasswordResetRequest passwordResetRequest);
        Task<(string, int)> ChangePassword(int playerId, string newPassword);
        Task<Player> GetPlayerVerified(int playerId, string hash);
    }
}
