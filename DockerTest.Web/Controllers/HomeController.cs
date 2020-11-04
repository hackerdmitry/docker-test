using DockerTest.Web.Controllers.Common;
using Microsoft.AspNetCore.Mvc;

namespace DockerTest.Web.Controllers
{
    [Route("")]
    [Route("home")]
    public class HomeController : BaseController
    {
        [HttpGet]
        [Route("")]
        [Route("index")]
        public ActionResult Index()
        {
            return View("Index");
        }
    }
}