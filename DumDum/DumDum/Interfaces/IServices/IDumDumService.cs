using System.Threading.Tasks;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Kingdom;
using DumDum.Models.JsonEntities.Player;

namespace DumDum.Interfaces
{
    public interface IDumDumService
    {
        PlayerResponse RegisterPlayerLogic(PlayerRequest playerRequest, out int statusCode);
        string RegisterKingdom(string authorization, KingdomRegistrationRequest kingdomRequest, out int statusCode);
        Task<KingdomsListResponse> GetAllKingdoms();
        Task<Player> GetPlayerByUsername(string username);
        Kingdom GetKingdomById(int kingdomId);
        Task<int> GetGoldAmountOfKingdom(int kingdomId);
        void TakeGold(int kingdomId, int amount);
        void TakeFood(int kingdomId, int amount);
        Player GetPlayerById(int id);
        Task<Location> AddLocations(Kingdom kingdom);
        Task<int> GetFoodAmountOfKingdom(int kingdomId);
        void GiveGold(int kingdomId, int amount);
        void GiveFood(int kingdomId, int amount);
        Kingdom RegisterKingdomToDB(int coordinateX, int coordinateY, int kingdomId);
        bool IsKingdomIdValid(int kingdomId);
        Task<bool> AreCredentialsValid(string username, string password);
        Task<bool> AreCoordinatesValid(int coordinateX, int coordinateY);
        bool DoCoordinatesExist(int coordinateX, int coordinateY);
        Task<Kingdom> CreateKingdom(string kingdomname, string username);
        Task<Player> Register(string username, string password, string kingdomName, string email);
        Task<Kingdom> GetKingdomByName(string kingdomName);
        string SetAuthToTrue(int playerId, string hash, out int statusCode);
        Task<string> ResetPassword(PasswordResetRequest passwordResetRequest, out int statusCode);
        string ChangePassword(int playerId, string newPassword, out int statusCode);
        Player GetPlayerVerified(int playerId, string hash);
    }
}
