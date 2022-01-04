using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using DumDum.Database;
using DumDum.Models;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Kingdom;
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
                var responseEnt = new AuthResponse(FindPlayerByTokenName(identity));
                return responseEnt;
            }
            catch (Exception)
            {
                return new AuthResponse();
            }
        }

        public KingdomRenameResponse RenameKingdom(KingdomRenameRequest requestKingdomName, AuthResponse authResponse)
        {
            KingdomRenameResponse response = new KingdomRenameResponse(FindPlayerByTokenName(authResponse.Ruler), requestKingdomName);
            DbContext.SaveChanges();
            return response;
        }
    
    }
}