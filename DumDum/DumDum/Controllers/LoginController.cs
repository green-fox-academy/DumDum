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

        public LoginController(LoginService service)
        {
            Service = service;
        }
        
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest player, LoginResponse response)
        {
            int statusCode;
            var message = Service.Login(player, response, out statusCode);

            if (statusCode == 200)
            {
                return Ok(new { status = "Ok", token = message });
            }

            return StatusCode(statusCode, new { error = message });
        }
    }
}
