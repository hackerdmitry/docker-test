using Microsoft.AspNetCore.Mvc;

namespace GuidWriter.Controllers
{
    [Route("")]
    public class IndexController : Controller
    {
        [Route("")]
        [Route("index")]
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Guid");
        }
    }
}