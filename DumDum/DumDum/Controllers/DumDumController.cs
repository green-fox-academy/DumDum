using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DumDum.Database;
using Microsoft.AspNetCore.Authorization;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Services;
using Microsoft.EntityFrameworkCore;

namespace DumDum.Controllers
{
    public class DumDumController : Controller
    {
        private DumDumService DumDumService { get; set; }
        public AuthenticateService AuthenticateService { get; set; }
        public ApplicationDbContext DbContext { get; set; }

        public DumDumController(DumDumService dumDumService, AuthenticateService authenticateService, ApplicationDbContext dbContext)
        {
            DumDumService = dumDumService;
            AuthenticateService = authenticateService;
            DbContext = dbContext;
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

        [HttpPut("kingdoms")]
        public IActionResult RenameKingdom([FromBody] KingdomRenameRequest requestName, [FromHeader] string authorization)
        {
            AuthRequest request = new AuthRequest(){Token = authorization};
            var player = AuthenticateService.GetUserInfo(request);
            if (player != null)
            {
                if ( !String.IsNullOrEmpty(requestName.KingdomName))
                {
                    var response = AuthenticateService.RenameKingdom(requestName, player);
                    return Ok(response);
                }
                return BadRequest(new { error = "Field kingdomName was empty!"});
            }
            return Unauthorized(new {error = "This kingdom does not belong to authenticated player"});
        }

        [HttpGet("kingdoms/{id}")]
        public IActionResult KingdomDetails([FromQuery] int id, [FromHeader] string authorization)
        {
            AuthRequest request = new AuthRequest(){Token = authorization};
            var player = AuthenticateService.GetUserInfo(request);
        }
    }
}
