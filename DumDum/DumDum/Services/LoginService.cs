using DumDum.Interfaces;
using DumDum.Models;
using DumDum.Models.JsonEntities.Login;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web.Helpers;

namespace DumDum.Services
{
    public class LoginService : ILoginService
    {
        private IDumDumService DumDumService { get; set; }
        private readonly AppSettings AppSettings;
        private IUnitOfWork UnitOfWork { get; set; }

        public LoginService(IOptions<AppSettings> appSettings,
            IDumDumService dumDumService, IUnitOfWork unitOfWork)
        {
            AppSettings = appSettings.Value;
            DumDumService = dumDumService;
            UnitOfWork = unitOfWork;
        }

        public bool LoginCheck(string username)
        {
            return UnitOfWork.Players.Any(x => x.Username == username);
        }

        public bool PasswordCheck(string password)
        {
            return UnitOfWork.Players.Any(x => x.Password == password);
        }

        public bool LoginPasswordCheck(string username, string password)
        {
            var playerFromDb = DumDumService.GetPlayerByUsername(username);
            var verified = Crypto.VerifyHashedPassword(playerFromDb.Password, password);
            return UnitOfWork.Players.Any(x => x.Username == username) && verified;
        }

        public string Authenticate(string username, string password)
        {
            if (LoginPasswordCheck(username, password))
            {
                //je potreba nainstalovat nuget System.IdentityModel.Tokens.Jwt
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.ASCII.GetBytes(AppSettings.Key);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, username), //z ceho ma brat jmeno
                    }),
                    Expires = DateTime.UtcNow.AddHours(24), //za kolik hodin vyprsi
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey),
                        SecurityAlgorithms.HmacSha256Signature) //typ tokenu
                };
                var token = tokenHandler.CreateToken(tokenDescriptor); //toto vytvori novy token
                return tokenHandler.WriteToken(token); //toto posle hotovy token ve stringu
            }
            else
            {
                return null;
            }
        }

        //logika pro controller
        public string Login(LoginRequest player, out int statusCode)
        {
            LoginResponse response = new LoginResponse();
            response.Token = Authenticate(player.Username, player.Password);

            if (string.IsNullOrEmpty(player.Username) || string.IsNullOrEmpty(player.Password))
            {
                statusCode = 400;
                return "Field username and/or field password was empty!";
            }

            if (!LoginPasswordCheck(player.Username, player.Password))
            {
                statusCode = 401;
                return "Username and/or password was incorrect!";
            }

            statusCode = 200;
            return response.Token;
        }
    }
}