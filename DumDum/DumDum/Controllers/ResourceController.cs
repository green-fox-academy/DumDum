using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DumDum.Controllers
{
    public class ResourceController : Controller
    {
        private ResourceService ResourceService { get; set; }
        private DumDumService DumDumService { get; set; }

        public ResourceController(DumDumService dumDumService, ResourceService resourceService)
        {
            DumDumService = dumDumService;
            ResourceService = resourceService;
        }
       
        [Route("")]
        [AllowAnonymous]
        [HttpGet("kingdoms/{id=int}/resources")]
        public IActionResult Resources([FromRoute] int id)
        {
            var kingdom = DumDumService.GetKingdomById(id);
            var player = DumDumService.GetPlayerById(kingdom.PlayerId);

            if (player.PlayerId != kingdom.PlayerId)
            {
                return Unauthorized(new {error = "This kingdom does not belong to authenticated player"});
            }
            
            return Ok(new ResourceResponse()
            {
                KingdomId = kingdom.KingdomId, KingdomName = kingdom.KingdomName,
                Ruler = player.Username, Resources = kingdom.Resources,
                Location = {CoordinateX = kingdom.CoordinateX, CoordinateY = kingdom.CoordinateY}
            });
        }
    }
}