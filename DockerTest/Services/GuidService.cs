using System;

namespace DockerTest.Services
{
    public class GuidService
    {
        public string Generate()
        {
            return Guid.NewGuid().ToString();
        }
    }
}