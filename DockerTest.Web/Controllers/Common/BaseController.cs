using Microsoft.AspNetCore.Mvc;

namespace DockerTest.Web.Controllers.Common
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("{controller}")]
    public class BaseController : Controller { }
}