using GuidWriter.Services;
using Microsoft.AspNetCore.Mvc;

namespace GuidWriter.Controllers
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
        public ActionResult IndexView()
        {
            return View("Index", _guidService.Generate());
        }
    }
}