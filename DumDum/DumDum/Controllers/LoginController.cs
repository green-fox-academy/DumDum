using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Login;
using DumDum.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DumDum.Controllers
{
    public class LoginController : Controller
    {
        private LoginService LoginService { get; set; }
        private TimeService TimeService { get; set; }
        public AuthenticateService AuthenticateService { get; set; }

        public LoginController(LoginService service, AuthenticateService auservice, TimeService timeService)
        {
            LoginService = service;
            AuthenticateService = auservice;
            TimeService = timeService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest player)
        {
            int statusCode;
            var message = LoginService.Login(player, out statusCode);
            

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
