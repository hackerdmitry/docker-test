using System;
using Microsoft.AspNetCore.Mvc;

namespace GuidWriter.Controllers
{
    [ApiController]
    [Route("")]
    [Route("Guid")]
    public class GuidController : ControllerBase
    {
        [Route("")]
        [Route("Write")]
        public string Write()
        {
            return Guid.NewGuid().ToString();
        }
    }
}