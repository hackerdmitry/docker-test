using System;

namespace GuidWriter.Services
{
    public class GuidService
    {
        public string Generate()
        {
            return Guid.NewGuid().ToString();
        }
    }
}