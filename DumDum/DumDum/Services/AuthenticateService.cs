using DumDum.Interfaces;
using DumDum.Models;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Kingdom;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using DumDum.Models.JsonEntities.Player;
using Microsoft.IdentityModel.Protocols;

namespace DumDum.Services
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly AppSettings _appSettings;
        private IUnitOfWork UnitOfWork { get; set; }

        public AuthenticateService(IOptions<AppSettings> appSettings, IUnitOfWork unitOfWork)
        {
            _appSettings = appSettings.Value;
            UnitOfWork = unitOfWork;
        }

        public Player FindPlayerByTokenName(string userName)
        {
            return UnitOfWork.Players.GetPlayerByUsername(userName).Result;
        }
        
        public async Task<AuthResponse> GetUserInfo(AuthRequest request)
        {
            // funkce smaže slovo bearer z tokenu, v případe, že by jej tam uživatel v postmanu zadal.
            var firstFive = request.Token.Substring(0, 6);
            if (firstFive == "bearer" || firstFive == "Bearer")
            {
                string token = request.Token;
                request.Token = token.Remove(0, 7);
            }
            var responseEnt = new AuthResponse();
            
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(request.Token) as JwtSecurityToken;

                if (jwtToken == null)
                    return null;

                var symmetricKey = Encoding.ASCII.GetBytes(_appSettings.Key);

                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
                };

                var principal = tokenHandler.ValidateToken(request.Token, validationParameters, out _);
                
                var identity = principal.Identity.Name;    //vraci username tokenu
                var responseEnt = new AuthResponse(FindPlayerByTokenName(identity));
                return responseEnt;
            }

            catch (Exception)
            {
                return new AuthResponse();
            }
        }

        public async Task<KingdomRenameResponse> RenameKingdom(KingdomRenameRequest requestKingdomName, AuthResponse authResponse)
        {
            var player = FindPlayerByTokenName(authResponse.Ruler);
            player.Kingdom.KingdomName = requestKingdomName.KingdomName;
            UnitOfWork.Complete();
            KingdomRenameResponse response = new KingdomRenameResponse(player);
            
            return response;
        }

        public bool IsEmailValid(string email)
        {
            return email.Contains("@") && email.Contains(".") && !UnitOfWork.Players.EmailNotUsed(email);
        }

        public void SendAccountVerificationEmail(Player player)
        {
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress("dumdumnya@gmail.com", "DumDum");
            mail.To.Add(new MailAddress(player.Email));

            mail.Subject = "Account verification";
            mail.IsBodyHtml = true;
            mail.Body =
                $"Welcome to DumDum, {player.Username}!\n The last step of registration is email verification.\n " +
                $"All you need is to click <a href=\"http://localhost:20625/emailAuthenticated/{player.PlayerId}?hash={player.Password}\">this link</a>";
            mail.Priority = MailPriority.High;
            var loginInfo = new NetworkCredential("dumdumnya@gmail.com", "dumdumcatcatcat");
            
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = loginInfo;
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }


        public void SendPasswordResetEmail(Player player)
        {
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress("dumdumnya@gmail.com", "DumDum");
            mail.To.Add(new MailAddress(player.Email));

            mail.Subject = "Password Reset";
            mail.IsBodyHtml = true;
            mail.Body =
                $"Hello, {player.Username}! You have requested for password reset recently. \n " +
                "If you still want to change your password, please, click " +
                $"<a href =\"http://localhost:20625/passwordChange/{player.PlayerId}?hash={player.Password}\">this link</a>";
            mail.Priority = MailPriority.Normal;
            var loginInfo = new NetworkCredential("dumdumnya@gmail.com", "dumdumcatcatcat");

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = loginInfo;
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }
    }
}