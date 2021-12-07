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
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest player,LoginResponse response)
        {
            response.Token = Service.Authenticate(player.Username, player.Password);
            if (string.IsNullOrEmpty(player.Username))
            {
                return StatusCode(400, new { error = "Field username and/or field password was empty!" });
            }
            if (!Service.LoginPasswordCheck(player.Username, player.Password))
            {
                return StatusCode(401, new { error = "Username and / or password was incorrect!" });
            }
                return Ok(new { status = "Ok", token = $"{response.Token}" });
        }
    }
}
