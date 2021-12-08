using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DumDum.Controllers
{
    public class LoginController : Controller
    {
        private LoginService LoginService { get; set; }
        public AuthenticateService AuthenticateService { get; set; }

        public LoginController(LoginService service, AuthenticateService auservice)
        {
            LoginService = service;
            AuthenticateService = auservice;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest player,LoginResponse response)
        {
            response.Token = LoginService.Authenticate(player.Username, player.Password);
            if (string.IsNullOrEmpty(player.Username) || string.IsNullOrEmpty(player.Password))
            {
                return StatusCode(400, new LoginResponse{ Error = "Field username and/or field password was empty!" });
            }
            if (!LoginService.LoginPasswordCheck(player.Username, player.Password))
            {
                return StatusCode(401, new LoginResponse { Error = "Username and/or password was incorrect!" });
            }
            return Ok(new LoginResponse{ Status = "Ok", Token = response.Token });
        }
        [HttpPost("auth")]
        public IActionResult TokenCheck([FromBody]AuthRequest request)
        {
            var response = AuthenticateService.GetUserInfo(request);
           
            if (response != null)
            {
                return Ok(response);
            }
            return BadRequest();
        }
    }
}
