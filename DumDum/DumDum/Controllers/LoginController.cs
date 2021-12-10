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
        private LoginService Service { get; set; }
        public AuthenticateService AuthenticateService { get; set; }

        public LoginController(LoginService service, AuthenticateService auservice)
        {
            Service = service;
            AuthenticateService = auservice;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest player)
        {
            int statusCode;
            var message = Service.Login(player, out statusCode);

            if (statusCode == 200)
            {
                return Ok(new LoginResponse{ Status = "Ok", Token = message });
            }
            return StatusCode(statusCode, new ErrorResponse{ Error = message });
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
