using Microsoft.AspNetCore.Mvc;

namespace DockerTest.Web.Controllers
{
    [Route("")]
    [Route("home")]
    public class HomeController : Controller
    {
        [Route("")]
        [Route("index")]
        public ActionResult Index()
        {
            return View("Index");
        }
    }
}