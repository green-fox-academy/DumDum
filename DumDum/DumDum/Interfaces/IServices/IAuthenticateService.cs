using System.Threading.Tasks;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Kingdom;

namespace DumDum.Interfaces
{
    public interface IAuthenticateService
    {
        Task<AuthResponse> GetUserInfo(AuthRequest request);
        Task<KingdomRenameResponse> RenameKingdom(KingdomRenameRequest requestKingdomName, AuthResponse authResponse);
        Task<Player> FindPlayerByTokenName(string userName);
        bool IsEmailValid(string email);
        void SendAccountVerificationEmail(Player player);
        void SendPasswordResetEmail(Player player);
    }
}
