using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly LoggingService _logging;

        public DepartmentController(
            MediScopeContext context,
            DepartmentRepository repo,
            ValidationService validator,
            LoggingService logging)
        {
            _context = context;
            _repo = repo;
            _validator = validator;
            _logging = logging;
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

            await _logging.AddAsync($"Admin created department (id={department.Id}, name={department.Name})");

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

            await _logging.AddAsync($"Admin edited department (id={dept.Id}, name={dept.Name})");
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

            await _logging.AddAsync($"Admin deleted department (id={dept.Id}, name={dept.Name})");

            return RedirectToAction("Index");
        }
    }
}
