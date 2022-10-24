using Microsoft.AspNetCore.Mvc;

namespace Rotom.Controllers
{
    [ApiController]
    [Attributes.ApiAuthenticationFilter]
    [Route("api/Data")]
    public class ApiController : Controller
    {
        private readonly Abstract.Services.ICurrentUserService _currentUserService;
        private readonly Abstract.Services.IDataService _dataService;

        public ApiController(Abstract.Services.ICurrentUserService currentUserService, Abstract.Services.IDataService dataService)
        {
            _currentUserService = currentUserService;
            _dataService = dataService;
        }

        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [Consumes("application/json")]
        public IActionResult DeliverData([FromBody] Models.ReportDataApiModel model)
        {
            int installationId = _currentUserService.GetCurrentInstallationId()!.Value;
            _dataService.SaveData(installationId, Util.Converters.Convert(model));
            return Ok();
        }
    }
}
