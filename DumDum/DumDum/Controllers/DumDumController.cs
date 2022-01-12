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
using DumDum.Models.Entities;

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
            int statusCode;
            var player = DumDumService.RegisterPlayerLogic(playerRequest, out statusCode);

            if (statusCode == 200)
            {
                return Ok(player);
            }

            return BadRequest(new ErrorResponse() { Error = "The credentials don't match required" });
        }

        [Authorize]
        [HttpPut("registration")]
        public IActionResult RegisterKingdom([FromHeader] string authorization, [FromBody] KingdomRegistrationRequest kingdomRequest)
        {
            var message = DumDumService.RegisterKingdom(authorization, kingdomRequest, out int statusCode);

            if (statusCode == 200)
            {
                return Ok(new StatusResponse { Status = "Ok" });
            }
            return StatusCode(statusCode, new ErrorResponse { Error = message });
        }

        [Authorize]
        [HttpPut("kingdoms")]
        public IActionResult RenameKingdom([FromBody] KingdomRenameRequest requestName, [FromHeader] string authorization)
        {
            AuthRequest request = new AuthRequest() { Token = authorization };
            var player = AuthenticateService.GetUserInfo(request);
            if (player == null)
            {
                return Unauthorized(new ErrorResponse { Error = "This kingdom does not belong to authenticated player" });
            }
            if (String.IsNullOrEmpty(requestName.KingdomName))
            {
                return BadRequest(new ErrorResponse { Error = "Field kingdomName was empty!" });
            }
            var response = AuthenticateService.RenameKingdom(requestName, player);
            return Ok(response);
        }

        [HttpGet("kingdoms")]
        public IActionResult KingdomsList()
        {
            var kingdoms = DumDumService.GetAllKingdoms();

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
            int statusCode;
            var details = DetailService.KingdomInformation(id, authorization, out statusCode);
            if (statusCode == 200)
            {
                return Ok(details);
            }
            return Unauthorized(new ErrorResponse { Error = "This kingdom does not belong to authenticated player" });
        }
        
        [AllowAnonymous]
        [HttpGet("emailAuthenticated/{playerId=int}")]
        public IActionResult PasswordReset([FromRoute] int playerId, [FromQuery] string hash)
        {
            int statusCode;
            var message = DumDumService.SetAuthToTrue(playerId, hash, out statusCode);

            if (statusCode == 200)
            {
                return Ok(message);
            }

            return BadRequest(new ErrorResponse() { Error = "Something went wrong" });
        }

        [AllowAnonymous]
        [HttpPost("passwordReset")]
        public IActionResult PasswordReset([FromBody] PasswordResetRequest passwordRequest)
        {
            int statusCode;
            var message = DumDumService.ResetPassword(passwordRequest, out statusCode);

            if (statusCode == 200)
            {
                return Ok(message);
            }

            return BadRequest(new ErrorResponse() { Error = "The credentials don't match required" });
        }
        
        [AllowAnonymous]
        [HttpPost("passwordChange")]
        public IActionResult PasswordChangeForm([FromForm] string newPassword, int playerId)
        {
            int statusCode;
            var message = DumDumService.ChangePassword(playerId,newPassword, out statusCode);

            if (statusCode == 200)
            {
                return Ok(message);
            }

            return BadRequest(new ErrorResponse() { Error = "The credentials don't match required" });
        }

       [AllowAnonymous]
        [HttpGet("passwordChange/{playerId=int}")]
        public IActionResult PasswordChange([FromRoute] int playerId,[FromQuery] string hash)
        {
            var player = DumDumService.GetPlayerVerified(playerId, hash);
            return View(player);
        }

    }
}