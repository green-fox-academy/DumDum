using DumDum.Interfaces;
using DumDum.Models;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Kingdom;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace DumDum.Services
{
    public class AuthenticateService
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
            return UnitOfWork.Players.GetPlayerByUsername(userName);
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
            KingdomRenameResponse response = new KingdomRenameResponse();
            var player = FindPlayerByTokenName(authResponse.Ruler);
            player.Kingdom.KingdomName = requestKingdomName.KingdomName;
            UnitOfWork.Complete();
            response.KingdomId = player.KingdomId;
            response.KingdomName = player.Kingdom.KingdomName;

            return response;
        }
    
    }
}