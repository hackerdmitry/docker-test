using System.Linq;
using System.Threading.Tasks;
using DockerTest.Data.Entities;
using DockerTest.Data.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace DockerTest.Web.Areas.Api
{
    [ApiController]
    [Route("api/link")]
    public class ApiLinkController : Controller
    {
        private readonly IRepository<Link> _linkRepository;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public ApiLinkController(IRepository<Link> linkRepository,
                                 IUnitOfWorkFactory unitOfWorkFactory)
        {
            _linkRepository = linkRepository;
            _unitOfWorkFactory = unitOfWorkFactory;
        }


        [HttpPut, Route("step")]
        public async Task<double> Update([FromQuery] int id)
        {
            var link = _linkRepository.GetAll().FirstOrDefault(x => x.Id == id);
            if (link == null || link.CurrentStep >= link.CountStep)
            {
                return -1;
            }

            using var uow = _unitOfWorkFactory.GetUoW();
            link.LinkStatus = LinkStatus.Processing;
            link.CurrentStep++;
            uow.Commit();

            if (link.CurrentStep == link.CountStep)
            {
                var client = new RestClient($"http://{link.Href}") {Timeout = -1};
                var request = new RestRequest(Method.GET);
                var response = await client.ExecuteAsync(request);
                link.Status = (int?) response.StatusCode;
                link.LinkStatus = LinkStatus.Done;
                uow.Commit();
                return -1;
            }

            return link.Tact;
        }
    }
}