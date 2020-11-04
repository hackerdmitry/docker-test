using DockerTest.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace DockerTest.Web.Areas.Api
{
    [ApiController]
    [Route("api/guid")]
    public class ApiGuidController : Controller
    {
        private readonly GuidService _guidService;

        public ApiGuidController()
        {
            _guidService = new GuidService();
        }

        [HttpGet, Route("")]
        public string Write()
        {
            return _guidService.Generate();
        }
    }
}