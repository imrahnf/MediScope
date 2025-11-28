/**************************************************************************
 * File: DepartmentController.cs
 * Author: Maryam Elhamidi
 *
 * Description:
 *     Implements full CRUD (Create, Read, Update, Delete) operations for the 
 *     Department module of the MediScope Admin system. Departments are used 
 *     to categorize doctors and manage medical organizational structure.
 *
 *     Features implemented by Maryam:
 *         • Department listing page (Index)
 *         • Department creation (GET/POST)
 *         • Department editing (GET/POST)
 *         • Department deletion with validation
 *         • Role-based access control (Admin-only)
 *         • Server-side validation using ValidationService
 *         • Audit logging using LoggingService
 *         • Data access through custom DepartmentRepository
 *
 * Last Modified: Nov 26, 2025
 **************************************************************************/

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediScope.Models;
using MediScope.Repositories;
using MediScope.Services;

namespace MediScope.Controllers
{
    /// <summary>
    /// Handles all administrative operations related to Department management.
    /// This includes listing, adding, editing, and deleting departments.
    /// Only Administrators may access this controller.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class DepartmentController : Controller
    {
        private readonly MediScopeContext _context;
        private readonly DepartmentRepository _repo;
        private readonly ValidationService _validator;
        private readonly LoggingService _logging;

        /// <summary>
        /// Constructor injecting DbContext, Repository, Validation, and Logging services.
        /// </summary>
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

        // --------------------------------------------------------------------
        //  LIST DEPARTMENTS
        // --------------------------------------------------------------------

        /// <summary>
        /// Displays a list of all departments in the system.
        /// Uses DepartmentRepository for data access.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var model = await _repo.GetAll();
            return View("Index", model);
        }

        // --------------------------------------------------------------------
        //  CREATE DEPARTMENT (GET)
        // --------------------------------------------------------------------

        /// <summary>
        /// Displays the form to create a new department.
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            return View("Create");
        }

        // --------------------------------------------------------------------
        //  CREATE DEPARTMENT (POST)
        // --------------------------------------------------------------------

        /// <summary>
        /// Creates a new department after validating the input through ValidationService.
        /// Logs the creation and redirects back to the Index page.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            // Validate name before creating a department
            var result = _validator.ValidateDepartment(name);
            if (!result.Success)
            {
                ViewBag.Error = result.Message;
                return View("Create");
            }

            var department = new Department { Name = name };

            await _repo.Add(department);
            await _repo.Save();

            // Log creation for audit history
            await _logging.AddAsync(
                $"Admin created department (id={department.Id}, name={department.Name})");

            return RedirectToAction("Index");
        }

        // --------------------------------------------------------------------
        //  EDIT DEPARTMENT (GET)
        // --------------------------------------------------------------------

        /// <summary>
        /// Loads the edit page for an existing department.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dept = await _repo.GetById(id);
            if (dept == null) return NotFound();

            return View("Edit", dept);
        }

        // --------------------------------------------------------------------
        //  EDIT DEPARTMENT (POST)
        // --------------------------------------------------------------------

        /// <summary>
        /// Updates the name of an existing department after validation.
        /// Logs the edit and saves to the database.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Edit(int id, string name)
        {
            var dept = await _repo.GetById(id);
            if (dept == null) return NotFound();

            // validate updated name
            var result = _validator.ValidateDepartment(name);
            if (!result.Success)
            {
                ViewBag.Error = result.Message;
                return View("Edit", dept);
            }

            dept.Name = name;

            await _repo.Save();

            // Log update
            await _logging.AddAsync(
                $"Admin edited department (id={dept.Id}, name={dept.Name})");

            return RedirectToAction("Index");
        }

        // --------------------------------------------------------------------
        //  DELETE DEPARTMENT
        // --------------------------------------------------------------------

        /// <summary>
        /// Deletes a department from the system.
        /// If the department does not exist, returns NotFound().
        /// Logs deletion for audit tracking.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var dept = await _repo.GetById(id);
            if (dept == null) return NotFound();

            _repo.Delete(dept);
            await _repo.Save();

            // Log deletion
            await _logging.AddAsync(
                $"Admin deleted department (id={dept.Id}, name={dept.Name})");

            return RedirectToAction("Index");
        }
    }
}
