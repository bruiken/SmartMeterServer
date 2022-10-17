using Microsoft.AspNetCore.Mvc;

namespace Rotom.Controllers
{
    [Attributes.AuthenticationFilter]
    [Route("")]
    public class HomeController : Controller
    {
        public const string Name = "Home";
        public static class Actions
        {
            public const string Index = "Index";
            public const string Error = "Error";
        }
        public HomeController()
        {
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet]
        [Route("Error")]
        public IActionResult Error()
        {
            return View();
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
