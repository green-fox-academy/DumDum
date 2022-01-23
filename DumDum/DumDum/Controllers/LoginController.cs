using DumDum.Interfaces.IServices;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DumDum.Controllers
{
    public class LoginController : Controller
    {
        private ILoginService LoginService { get; set; }
        private IAuthenticateService AuthenticateService { get; set; }

        public LoginController(ILoginService service, IAuthenticateService auservice)
        {
            LoginService = service;
            AuthenticateService = auservice;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest player)
        {
            var message = LoginService.Login(player).Result;

            if (message.Item2 == 200)
            {
                return Ok(new LoginResponse{ Status = "Ok", Token = message.Item1 });
            }
            return StatusCode(message.Item2, new ErrorResponse{ Error = message.Item1 });
        }
        [HttpPost("auth")]
        public IActionResult TokenCheck([FromBody]AuthRequest request)
        {
            var response = AuthenticateService.GetUserInfo(request).Result;
           
            if (response != null)
            {
                return Ok(response);
            }
            return BadRequest();
        }
    }
}
