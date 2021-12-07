using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using DumDum.Database;
using DumDum.Models;
using DumDum.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DumDum.Services
{
    public class AuthenticateService
    {
        private ApplicationDbContext DbContext { get; set; }
        private readonly AppSettings _appSettings;

        public AuthenticateService(ApplicationDbContext dbContex,IOptions<AppSettings> appSettings)
        {
            DbContext = dbContex;
            _appSettings = appSettings.Value;
        }

        public Player FindPlayerByTokenName(string tokenName)
        {
            var player = DbContext.Players.Include(p =>p.KingdomId).FirstOrDefault(p => p.Username == tokenName);
            return player;
        }
        
        public List<string> GetPrincipal(string token)
        {
            List<string> UserInfo = new List<string>();
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

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

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                

                var identity = principal.Identity.Name;    //vraci username tokenu
                var player = FindPlayerByTokenName(identity);
                UserInfo.Add(player.Username);
                UserInfo.Add(player.KingdomId.ToString());
                UserInfo.Add(player.Kingdom.KingdomName);

                return UserInfo;
            }

            catch (Exception)
            {
                return null;
            }
        }
    
    }
}