using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediScope.Models;
using MediScope.Repositories;
using MediScope.Services;

namespace MediScope.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DepartmentController : Controller
    {
        private readonly MediScopeContext _context;
        private readonly DepartmentRepository _repo;
        private readonly ValidationService _validator;

        public DepartmentController(
            MediScopeContext context,
            DepartmentRepository repo,
            ValidationService validator)
        {
            _context = context;
            _repo = repo;
            _validator = validator;
        }

        // LIST
        public async Task<IActionResult> Index()
        {
            var model = await _repo.GetAll();
            return View("Index", model);
        }

        // CREATE
        [HttpGet]
        public IActionResult Create()
        {
            return View("Create");
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            var result = _validator.ValidateDepartment(name);
            if (!result.Success)
            {
                ViewBag.Error = result.Message;
                return View("Create");
            }

            var department = new Department { Name = name };
            await _repo.Add(department);
            await _repo.Save();

            return RedirectToAction("Index");
        }

        // EDIT
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dept = await _repo.GetById(id);
            if (dept == null) return NotFound();

            return View("Edit", dept);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, string name)
        {
            var dept = await _repo.GetById(id);
            if (dept == null) return NotFound();

            var result = _validator.ValidateDepartment(name);
            if (!result.Success)
            {
                ViewBag.Error = result.Message;
                return View("Edit", dept);
            }

            dept.Name = name;
            await _repo.Save();
            return RedirectToAction("Index");
        }

        // DELETE
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var dept = await _repo.GetById(id);
            if (dept == null) return NotFound();

            _repo.Delete(dept);
            await _repo.Save();

            return RedirectToAction("Index");
        }
    }
}
