using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediScope.Models;
using MediScope.Repositories;
using MediScope.Services;
using Microsoft.EntityFrameworkCore;

namespace MediScope.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ResourceController : Controller
    {
        private readonly ResourceRepository _repo;
        private readonly ValidationService _validator;
        private readonly LoggingService _logging;

        public ResourceController(ResourceRepository repo, ValidationService validator, LoggingService logging)
        {
            _repo = repo;
            _validator = validator;
            _logging = logging;
        }
        // LIST
        public async Task<IActionResult> Index()
        {
            var items = await _repo.GetAll();
            return View("Index", items);
        } 
        // CREATE
        [HttpGet]
        public IActionResult Create()
        {
            return View("Create");
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name, string type, int quantity)
        {
            var result = _validator.ValidateResource(name, type, quantity);
            if (!result.Success)
            {
                ViewBag.Error = result.Message;
                return View("Create");
            }

            var res = new Resource
            {
                Name = name,
                Type = type,
                Quantity = quantity
            };

            await _repo.Add(res);
            await _repo.Save();

            await _logging.AddAsync($"Admin created resource (id={res.Id}, name={res.Name})");

            return RedirectToAction("Index");
        }
        // EDIT
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var res = await _repo.GetById(id);
            if (res == null) return NotFound();

            return View("Edit", res);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, string name, string type, int quantity)
        {
            var res = await _repo.GetById(id);
            if (res == null) return NotFound();

            var result = _validator.ValidateResource(name, type, quantity);
            if (!result.Success)
            {
                ViewBag.Error = result.Message;
                return View("Edit", res);
            }

            res.Name = name;
            res.Type = type;
            res.Quantity = quantity;

            await _repo.Save();

            await _logging.AddAsync($"Admin edited resource (id={res.Id}, name={res.Name})");
            return RedirectToAction("Index");
        }
        // DELETE
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _repo.GetById(id);
            if (res == null) return NotFound();

            _repo.Delete(res);
            await _repo.Save();

            await _logging.AddAsync($"Admin deleted resource (id={res.Id}, name={res.Name})");

            return RedirectToAction("Index");
        }
    }
}
