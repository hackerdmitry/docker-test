using System.Threading.Tasks;
using Autofac;

namespace Jobs.LinkConsumer
{
    static class Program
    {
        private static async Task Main()
        {
            var startup = new Startup();
            var container = startup.Initialize();

            await container.Resolve<LinkHandler>().HandleAsync();
        }
    }
}