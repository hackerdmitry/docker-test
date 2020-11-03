using System;
using System.Linq;
using System.Threading.Tasks;
using DockerTest.Data.Entities;
using DockerTest.Data.Events;
using DockerTest.Data.Infrastructure.Interfaces;
using DockerTest.Web.Configurations;
using DockerTest.Web.Services;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace DockerTest.Web.Controllers
{
    [Route("link")]
    public class LinkController : Controller
    {
        private readonly IRepository<Link> _linkRepository;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly RabbitMqService _rabbitMqService;

        public LinkController(IRepository<Link> linkRepository,
                              IUnitOfWorkFactory unitOfWorkFactory,
                              RabbitMqConfiguration rabbitMqConfiguration)
        {
            _linkRepository = linkRepository;
            _unitOfWorkFactory = unitOfWorkFactory;
            _rabbitMqService = new RabbitMqService(rabbitMqConfiguration);
        }

        [HttpGet]
        public ActionResult Index()
        {
            var links = _linkRepository.GetAll()
               .OrderBy(x => x.LinkStatus == LinkStatus.Done ? 1 : 0)
               .ThenBy(x => x.Id)
               .ToArray();
            return View("Index", links);
        }

        [HttpPost, Route("add")]
        public ActionResult Add(string href, int countStep, double tact)
        {
            if (href == null) return UnprocessableEntity("Поле является обязательным");
            if (href.Length > 50) return UnprocessableEntity("Ссылка должна быть не больше 50 символов");
            if (countStep < 1) return UnprocessableEntity("Количество шагов должно быть больше 1");
            if (tact < 0) return UnprocessableEntity("Такт должен быть неотрицательным");

            try
            {
                new Uri($"http://{href}");
            }
            catch (UriFormatException e)
            {
                return UnprocessableEntity(e.Message);
            }

            using var uow = _unitOfWorkFactory.GetUoW();
            var link = new Link
            {
                Href = href,
                CountStep = countStep,
                Tact = tact,
                LinkStatus = LinkStatus.Waiting
            };
            _linkRepository.Add(link);
            uow.Commit();

            var linkEvent = new LinkEvent{Id = link.Id};
            var successfullySent = _rabbitMqService.SendLinkEvent(linkEvent);
            link.LinkStatus = successfullySent ? LinkStatus.Queue : LinkStatus.Waiting;
            uow.Commit();

            return RedirectToAction("Index");
        }

        [HttpPut, Route("step")]
        public async Task<double> Update(int id)
        {
            var link = _linkRepository.GetAll().FirstOrDefault(x => x.Id == id);
            if (link.CurrentStep >= link.CountStep)
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
                return link.Tact;
            }

            return -1;
        }

        [HttpDelete, Route("delete")]
        public void Delete(int id)
        {
            var link = _linkRepository.GetAll().FirstOrDefault(x => x.Id == id);
            if (link == null)
            {
                throw new ArgumentException("Такого объявления не существует");
            }

            using var uow = _unitOfWorkFactory.GetUoW();
            _linkRepository.Remove(link);
            uow.Commit();
        }
    }
}