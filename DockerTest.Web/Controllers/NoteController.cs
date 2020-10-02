using System;
using System.Linq;
using DockerTest.Data.Entities;
using DockerTest.Data.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DockerTest.Web.Controllers
{
    [Route("note")]
    public class NoteController : Controller
    {
        private readonly IRepository<Note> _noteRepository;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public NoteController(IRepository<Note> noteRepository,
                                IUnitOfWorkFactory unitOfWorkFactory)
        {
            _noteRepository = noteRepository;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var notes = _noteRepository.GetAll().ToArray();
            return View("Index", notes);
        }

        [HttpPost, Route("add")]
        public ActionResult Add(string description)
        {
            if (description == null)
            {
                return UnprocessableEntity("Поле является обязательным");
            }

            if (description.Length > 50)
            {
                return UnprocessableEntity("Текст должен быть не больше 50 символов");
            }

            using var uow = _unitOfWorkFactory.GetUoW();
            var note = new Note{Description = description};
            _noteRepository.Add(note);
            uow.Commit();

            return RedirectToAction("Index");
        }

        [HttpDelete, Route("delete")]
        public void Delete(int id)
        {
            var note = _noteRepository.GetAll().FirstOrDefault(x => x.Id == id);
            if (note == null)
            {
                throw new ArgumentException("Такого объявления не существует");
            }

            using var uow = _unitOfWorkFactory.GetUoW();
            _noteRepository.Remove(note);
            uow.Commit();
        }
    }
}