using Microsoft.AspNetCore.Mvc;

namespace DockerTest.Controllers
{
    [Route("")]
    [Route("home")]
    public class HomeController : Controller
    {
        [Route("")]
        [Route("index")]
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Guid");
        }
    }
}