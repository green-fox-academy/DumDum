using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Authorization;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Kingdom;
using DumDum.Models.JsonEntities.Player;
using DumDum.Services;
using Newtonsoft.Json;
using DumDum.Interfaces;

namespace DumDum.Controllers
{
    public class DumDumController : Controller
    {
        private DumDumService DumDumService { get; set; }
        private AuthenticateService AuthenticateService { get; set; }
        private DetailService DetailService { get; set; }
        private IUnitOfWork UnitOfWork { get; set; }
        private TimeService TimeService { get; set; }

        public DumDumController(DumDumService dumDumService, AuthenticateService authenticateService, DetailService detailService, IUnitOfWork unitOfWork, TimeService timeService)
        {
            DumDumService = dumDumService;
            AuthenticateService = authenticateService;
            DetailService = detailService;
            UnitOfWork = unitOfWork;
            TimeService = timeService;
        }

        [AllowAnonymous]
        [HttpPost("registration")]
        public IActionResult Register([FromBody] PlayerRequest playerRequest)
        {
            var player = DumDumService.RegisterPlayerLogic(playerRequest).Result;

            if (player.Item2 == 200)
            {
                return Ok(player.Item1);
            }

            return BadRequest(new ErrorResponse() { Error = "The credentials don't match required" });
        }

        [Authorize]
        [HttpPut("registration")]
        public IActionResult RegisterKingdom([FromHeader] string authorization, [FromBody] KingdomRegistrationRequest kingdomRequest)
        {
            var message = DumDumService.RegisterKingdom(authorization, kingdomRequest).Result;

            if (message.Item2 == 200)
            {
                return Ok(new StatusResponse { Status = "Ok" });
            }
            return StatusCode(message.Item2, new ErrorResponse { Error = message.Item1 });
        }

        [Authorize]
        [HttpPut("kingdoms")]
        public IActionResult RenameKingdom([FromBody] KingdomRenameRequest requestName, [FromHeader] string authorization)
        {
            AuthRequest request = new AuthRequest() { Token = authorization };
            var player = AuthenticateService.GetUserInfo(request).Result;
            if (player == null)
            {
                return Unauthorized(new ErrorResponse { Error = "This kingdom does not belong to authenticated player" });
            }
            if (String.IsNullOrEmpty(requestName.KingdomName))
            {
                return BadRequest(new ErrorResponse { Error = "Field kingdomName was empty!" });
            }
            var response = AuthenticateService.RenameKingdom(requestName, player).Result;
            return Ok(response);
        }

        [HttpGet("kingdoms")]
        public IActionResult KingdomsList()
        {
            var kingdoms = DumDumService.GetAllKingdoms().Result;

            if (kingdoms == null)
            {
                return StatusCode(500);
            }
            return Ok(kingdoms);
        }

        [Authorize]
        [HttpGet("kingdoms/{id=int}")]
        public IActionResult KingdomDetails([FromRoute] int id, [FromHeader] string authorization)
        {
            var details = DetailService.KingdomInformation(id, authorization).Result;
            if (details.Item2 == 200)
            {
                return Ok(details.Item1);
            }
            return Unauthorized(new ErrorResponse { Error = "This kingdom does not belong to authenticated player" });
        }


    }
}