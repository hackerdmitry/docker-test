using DockerTest.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace DockerTest.Web.Controllers
{
    [Route("guid")]
    public class GuidController : Controller
    {
        private readonly GuidService _guidService;

        public GuidController()
        {
            _guidService = new GuidService();
        }

        [HttpGet, Route("view")]
        public ActionResult Index()
        {
            return View("Index", _guidService.Generate());
        }
    }
}