﻿using DumDum.Database;
using DumDum.Models;
using DumDum.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DumDum.Services
{
    public class LoginService
    {
        
        private ApplicationDbContext DbContext { get; set; }
        private readonly AppSettings _appSettings;

        public LoginService(ApplicationDbContext dbContex,IOptions<AppSettings> appSettings)
        {
            DbContext = dbContex;
            _appSettings = appSettings.Value;
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
                var tokenKey = Encoding.ASCII.GetBytes(_appSettings.Key);
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
        public string GetPrincipal(string token)
        {
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

                return identity;
            }

            catch (Exception)
            {
                return null;
            }
        }

        public IActionResult LoginLogic(Player player)
        {
            var token = Authenticate(player.Username, player.Password);
            if (string.IsNullOrEmpty(player.Username))
            {
                return StatusCode(400, new { error = "Field username and/or field password was empty!" });
            }
            else if (LoginPasswordCheck(player.Username, player.Password))
            {
                return StatusCode(401, new { error = "Username and / or password was incorrect!" });
            }
            else
            {
                return Ok(new { status = "Ok", token = $"{token}" });
            }
        }

        private IActionResult Ok(object p)
        {
            throw new NotImplementedException();
        }

        private IActionResult StatusCode(int v, object p)
        {
            throw new NotImplementedException();
        }
    }
}
