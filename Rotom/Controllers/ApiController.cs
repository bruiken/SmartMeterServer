using Microsoft.AspNetCore.Mvc;

namespace Rotom.Controllers
{
    [ApiController]
    [Attributes.ApiAuthenticationFilter]
    [Route("api/Data")]
    public class ApiController : Controller
    {
        private readonly Abstract.Services.ICurrentUserService _currentUserService;

        public ApiController(Abstract.Services.ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [Consumes("application/json")]
        public IActionResult DeliverData()
        {
            int installationId = _currentUserService.GetCurrentInstallationId()!.Value;

            return Ok();
        }
    }
}
