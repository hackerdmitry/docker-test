using DockerTest.Services;
using Microsoft.AspNetCore.Mvc;

namespace DockerTest.Areas.Api
{
    [ApiController]
    [Area("api")]
    [Route("guid")]
    public class ApiGuidController : Controller
    {
        private readonly GuidService _guidService;

        public ApiGuidController()
        {
            _guidService = new GuidService();
        }

        [Route("")]
        [Route("write")]
        public string Write()
        {
            return _guidService.Generate();
        }
    }
}