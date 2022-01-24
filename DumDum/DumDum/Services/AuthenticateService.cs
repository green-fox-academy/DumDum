using DumDum.Models;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Kingdom;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using DumDum.Interfaces.IRepositories;
using DumDum.Interfaces.IServices;
using SendGrid;
using SendGrid.Helpers.Mail;

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

        public async Task<Player> FindPlayerByTokenName(string userName)
        {
            return await UnitOfWork.Players.GetPlayerByUsername(userName);
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
                var responseEnt = new AuthResponse(await FindPlayerByTokenName(identity));
                return responseEnt;
            }

            catch (Exception)
            {
                return new AuthResponse();
            }
        }

        public async Task<KingdomRenameResponse> RenameKingdom(KingdomRenameRequest requestKingdomName, AuthResponse authResponse)
        {
            var player = await FindPlayerByTokenName(authResponse.Ruler);
            player.Kingdom.KingdomName = requestKingdomName.KingdomName;
            UnitOfWork.Complete();
            KingdomRenameResponse response = new KingdomRenameResponse(player);

            return response;
        }

        public async Task<bool> IsEmailValid(string email)
        {
            MailMessage mail = new MailMessage();
            try
            {
                mail.To.Add(email);
                return email.Contains("@") && email.Contains(".") && !UnitOfWork.Players.EmailNotUsed(email);
            }
            catch (Exception)
            {
                return false;
            }
        }

        //public async Task SendAccountVerificationEmail(Player player)
        //{
        //    MailMessage mail = new MailMessage();

        //    mail.From = new MailAddress("dumdumnya@gmail.com", "DumDum");
        //    mail.To.Add(new MailAddress(player.Email));

        //    mail.Subject = "Account verification";
        //    mail.IsBodyHtml = true;
        //    mail.Body =
        //        $"Welcome to DumDum, {player.Username}!\n The last step of registration is email verification.\n " +
        //        $"All you need is to click <a href=\"http://dumdumdumdum.azurewebsites.net/emailAuthenticated/{player.PlayerId}?hash={player.Password}\">this link</a>";
        //    mail.Priority = MailPriority.High;
        //   // var loginInfo = new NetworkCredential("dumdumnya@gmail.com", "dumdumcatcatcat");
        //    var loginInfo = CredentialCache.DefaultNetworkCredentials;


        //    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);

        //    smtp.Credentials = loginInfo;
        //    smtp.UseDefaultCredentials = true;
        //    smtp.EnableSsl = true;
        //    smtp.Send(mail);
        //}

        public async Task SendAccountVerificationEmail(Player player)
        {
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress("dumdumnya@gmail.com", "DumDum");
            mail.To.Add(new MailAddress(player.Email));

            mail.Subject = "Account verification";
            mail.IsBodyHtml = true;
            mail.Body =
                $"Welcome to DumDum, {player.Username}!\n The last step of registration is email verification.\n " +
                $"All you need is to click <a href=\"http://dumdumdumdum.azurewebsites.net/emailAuthenticated/{player.PlayerId}?hash={player.Password}\">this link</a>";
            mail.Priority = MailPriority.High;
            var loginInfo = new NetworkCredential("apikey", "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");
            //var loginInfo = CredentialCache.DefaultNetworkCredentials;
            SmtpClient smtp = new SmtpClient("smtp.sendgrid.net", 587);

           // smtp.UseDefaultCredentials = true;
            smtp.Credentials = loginInfo;
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }

        public async Task SendPasswordResetEmail(Player player)
        {
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress("dumdumnya@gmail.com", "DumDum");
            mail.To.Add(new MailAddress(player.Email));

            mail.Subject = "Password Reset";
            mail.IsBodyHtml = true;
            mail.Body =
                $"Hello, {player.Username}! You have requested for password reset recently. \n " +
                "If you still want to change your password, please, click " +
                 $"<a href =\"http://dumdumdumdum.azurewebsites.net/emailAuthenticated/{player.PlayerId}?hash={player.Password}\">this link</a>";

            mail.Priority = MailPriority.Normal;
            // var loginInfo = new NetworkCredential("dumdumnya@gmail.com", "dumdumcatcatcat");
            var loginInfo = new NetworkCredential("apikey", "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");

           // var loginInfo = CredentialCache.DefaultNetworkCredentials;

            SmtpClient smtp = new SmtpClient("smtp.sendgrid.net", 587);
           // smtp.UseDefaultCredentials = true;
            smtp.Credentials = loginInfo;
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }
    }
}