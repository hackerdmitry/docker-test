using System;
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
        public async Task<double> Update([FromQuery] int id,
                                         [FromBody] LinkResponse response)
        {
            Console.WriteLine($"Принято: id={id} response={response?.StatusCode}");

            using var uow = _unitOfWorkFactory.GetUoW();
            var link = _linkRepository.GetAll().FirstOrDefault(x => x.Id == id);
            if (link != null && response?.StatusCode != null)
            {
                link.Status = response.StatusCode;
                link.LinkStatus = LinkStatus.Done;
                uow.Commit();
                return -1;
            }

            if (link == null || link.CurrentStep >= link.CountStep)
            {
                return -1;
            }

            link.LinkStatus = LinkStatus.Processing;
            link.CurrentStep++;
            uow.Commit();

            return link.CurrentStep == link.CountStep
                ? -1
                : link.Tact;
        }

        public class LinkResponse
        {
            public int? StatusCode { get; set; }
        }
    }
}