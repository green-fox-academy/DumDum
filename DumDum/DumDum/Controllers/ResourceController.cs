using System.Linq;
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

        private AuthenticateService AuthenticateService { get; set; }

        public ResourceController(DumDumService dumDumService, ResourceService resourceService)
        {
            DumDumService = dumDumService;
            ResourceService = resourceService;
        }
       
        [Route("")]
        [Authorize]
        [HttpGet("kingdoms/{id=int}/resources")]
        public IActionResult Resources([FromRoute] int id, [FromHeader] string authorization)
        {
            int statusCode;
            var response = ResourceService.ResourceLogic(id, authorization, out statusCode);
            
            if (statusCode == 200)
            {
                return Ok(response);
            }
            return Unauthorized(new ErrorResponse(){Error = "This kingdom does not belong to authenticated player"});
        }
    }
}
