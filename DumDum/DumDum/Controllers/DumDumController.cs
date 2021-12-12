using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Services;

namespace DumDum.Controllers
{
    public class DumDumController : Controller
    {
        private DumDumService DumDumService { get; set; }
        public AuthenticateService AuthenticateService { get; set; }

        public DumDumController(DumDumService dumDumService, AuthenticateService authenticateService)
        {
            DumDumService = dumDumService;
            AuthenticateService = authenticateService;
        }

        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost("registration")]
        public IActionResult Register([FromBody] PlayerRequest playerRequest)
        {
            int statusCode;
            var player = DumDumService.RegisterPlayerLogic(playerRequest, out statusCode);

            if (statusCode == 200)
            {
                return Ok(player);
            }

            return BadRequest();
        }

        [AllowAnonymous]
        [HttpPut("registration")]
        public IActionResult RegisterKingdom([FromBody] KingdomJson kingdomJson)
        {
            int statusCode;
            var message = DumDumService.RegisterKingdomLogic(kingdomJson, out statusCode);

            if (statusCode == 200)
            {
                return Ok(new StatusResponse{ Status = "Ok" });
            }
            return StatusCode(statusCode, new ErrorResponse{ Error = message });
        }
        [Authorize]
        [HttpPut("kingdoms")]
        public IActionResult RenameKingdom([FromBody] KingdomRenameRequest requestName, [FromHeader] string authorization)
        {
            AuthRequest request = new AuthRequest(){Token = authorization};
            var player = AuthenticateService.GetUserInfo(request);
            if (player == null)
            {
                return Unauthorized(new {error = "This kingdom does not belong to authenticated player"});
            }
            if (String.IsNullOrEmpty(requestName.KingdomName))
            {
                return BadRequest(new { error = "Field kingdomName was empty!"});
            }
            var response = AuthenticateService.RenameKingdom(requestName, player);
            return Ok(response);
        }
    }
}
