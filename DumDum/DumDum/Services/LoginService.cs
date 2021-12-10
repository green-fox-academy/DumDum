using DumDum.Database;
using DumDum.Models;
using DumDum.Models.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DumDum.Models.JsonEntities;

namespace DumDum.Services
{
    public class LoginService
    {
        
        private ApplicationDbContext DbContext { get; set; }
        private readonly AppSettings AppSettings;

        public LoginService(ApplicationDbContext dbContex,IOptions<AppSettings> appSettings)
        {
            DbContext = dbContex;
            AppSettings = appSettings.Value;
        }

        public bool LoginCheck(string username)
        {
            return DbContext.Players.Any(x => x.Username == username);
        }
        public bool PasswordCheck(string password)
        {
            return DbContext.Players.Any(x => x.Password == password);
        }
        public bool LoginPasswordCheck(string username, string password)
        {
            return DbContext.Players.Any(x => x.Username == username && x.Password == password);
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
                        new Claim(ClaimTypes.Name, username),//z ceho ma brat jmeno
                    }),
                    Expires = DateTime.UtcNow.AddHours(24),  //za kolik hodin vyprsi
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature) //typ tokenu
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);  //toto vytvori novy token
                return tokenHandler.WriteToken(token);  //toto posle hotovy token ve stringu
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
