using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using DumDum.Database;
using DumDum.Models;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DumDum.Services
{
    public class AuthenticateService
    {
        private ApplicationDbContext DbContext { get; set; }
        private readonly AppSettings _appSettings;

        public AuthenticateService(ApplicationDbContext dbContext,IOptions<AppSettings> appSettings)
        {
            DbContext = dbContext;
            _appSettings = appSettings.Value;
        }

        public Player FindPlayerByTokenName(string userName)
        {
            var player = DbContext.Players.Include(p =>p.Kingdom).FirstOrDefault(p => p.Username == userName);
            return player;
        }
        
        public AuthResponse GetUserInfo(AuthRequest request)
        {
            var responseEnt = new AuthResponse();
            string token = request.Token;
            request.Token = token.Remove(0, 7);
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
                var player = FindPlayerByTokenName(identity);
                responseEnt.Ruler = player.Username;
                responseEnt.KingdomId = player.KingdomId;
                responseEnt.KingdomName = player.Kingdom.KingdomName;
                
                return responseEnt;
            }

            catch (Exception ex)
            {
                return null;
            }
        }

        public KingdomRenameResponse RenameKingdom(KingdomRenameRequest requestKingdomName, AuthResponse authResponse)
        {
            KingdomRenameResponse response = new KingdomRenameResponse();
            var player = FindPlayerByTokenName(authResponse.Ruler);
            player.Kingdom.KingdomName = requestKingdomName.KingdomName;
            DbContext.SaveChanges();
            response.KingdomId = player.KingdomId;
            response.KingdomName = player.Kingdom.KingdomName;

            return response;
        }
    
    }
}