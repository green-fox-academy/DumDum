using DumDum.Models;
using DumDum.Models.JsonEntities.Login;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using DumDum.Interfaces.IRepositories;
using DumDum.Interfaces.IServices;

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

        public async Task<bool> LoginCheck(string username)
        {
            return UnitOfWork.Players.Any(x => x.Username == username).Result;
        }

        public async Task<bool> PasswordCheck(string password)
        {
            return UnitOfWork.Players.Any(x => x.Password == password).Result;
        }

        public async Task<bool> LoginPasswordCheck(string username, string password)
        {
            var playerFromDb = await DumDumService.GetPlayerByUsername(username);
            var verified = Crypto.VerifyHashedPassword(playerFromDb.Password, password);
            return UnitOfWork.Players.Any(x => x.Username == username).Result && verified;
        }

        public async Task<string> Authenticate(string username, string password)
        {
            if (!await LoginPasswordCheck(username, password))
            {
                return null;
            }
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

        //logika pro controller
        public async Task<(string, int)>Login(LoginRequest player)
        {
            LoginResponse response = new LoginResponse();
            var playerTologin = await DumDumService.GetPlayerByUsername(player.Username);
            if (await LoginPasswordCheck(player.Username, player.Password) && playerTologin.IsVerified)
            {
                response.Token = await Authenticate(player.Username, player.Password);
            }

            if (string.IsNullOrEmpty(player.Username) || string.IsNullOrEmpty(player.Password))
            {
                return ("Field username and/or field password was empty!", 400);
            }

            if (! await LoginPasswordCheck(player.Username, player.Password))
            {
                return ("Username and/or password was incorrect!", 401);
            }
            return (response.Token, 200);
        }
    }
}