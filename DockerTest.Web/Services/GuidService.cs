using System;

namespace DockerTest.Web.Services
{
    public class GuidService
    {
        public string Generate()
        {
            return Guid.NewGuid().ToString();
        }
    }
}