using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Kingdom;

namespace DumDum.Interfaces
{
    public interface IAuthenticateService
    {
        AuthResponse GetUserInfo(AuthRequest request);
        KingdomRenameResponse RenameKingdom(KingdomRenameRequest requestKingdomName, AuthResponse authResponse);
        Player FindPlayerByTokenName(string userName);
        bool IsEmailValid(string email);
        void SendAccountVerificationEmail(Player player);
        void SendPasswordResetEmail(Player player);
    }
}
