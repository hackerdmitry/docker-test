using GuidWriter.Services;
using Microsoft.AspNetCore.Mvc;

namespace GuidWriter.Controllers
{
    [Route("")]
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