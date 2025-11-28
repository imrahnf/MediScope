/**************************************************************************
 * File: ResourceController.cs
 * Author: Maryam Elhamidi
 *
 * Description:
 *     Implements full CRUD functionality for the Resource Management module
 *     within the MediScope Admin panel. Resources represent hospital assets
 *     such as equipment, tools, or supplies that must be tracked and reported.
 *
 *     Features implemented by Maryam:
 *         • Resource listing view (Index)
 *         • Resource creation (GET + POST)
 *         • Resource editing (GET + POST)
 *         • Resource deletion with audit logging
 *         • Validation through ValidationService
 *         • Data access via ResourceRepository
 *         • Admin-only role-based restrictions
 *         • Action-level audit logging for every operation
 *
 * Last Modified: Nov 26, 2025
 **************************************************************************/

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediScope.Models;
using MediScope.Repositories;
using MediScope.Services;
using Microsoft.EntityFrameworkCore;

namespace MediScope.Controllers
{
    /// <summary>
    /// Handles all CRUD operations for Resources.
    /// Only Admins may access this controller.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class ResourceController : Controller
    {
        private readonly ResourceRepository _repo;
        private readonly ValidationService _validator;
        private readonly LoggingService _logging;

        /// <summary>
        /// Constructor injecting repository, validation, and logging services.
        /// </summary>
        public ResourceController(ResourceRepository repo, ValidationService validator, LoggingService logging)
        {
            _repo = repo;
            _validator = validator;
            _logging = logging;
        }

        // --------------------------------------------------------------------
        //  LIST RESOURCES
        // --------------------------------------------------------------------

        /// <summary>
        /// Displays a list of all hospital resources.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var items = await _repo.GetAll();
            return View("Index", items);
        }

        // --------------------------------------------------------------------
        //  CREATE RESOURCE (GET)
        // --------------------------------------------------------------------

        /// <summary>
        /// Loads the form for creating a new resource.
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            return View("Create");
        }

        // --------------------------------------------------------------------
        //  CREATE RESOURCE (POST)
        // --------------------------------------------------------------------

        /// <summary>
        /// Creates a new Resource entry after validation.
        /// Logs the creation for audit purposes.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(string name, string type, int quantity)
        {
            // Validate inputs before creating resource
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

            await _logging.AddAsync(
                $"Admin created resource (id={res.Id}, name={res.Name})");

            return RedirectToAction("Index");
        }

        // --------------------------------------------------------------------
        //  EDIT RESOURCE (GET)
        // --------------------------------------------------------------------

        /// <summary>
        /// Loads the edit form for an existing resource.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var res = await _repo.GetById(id);
            if (res == null) return NotFound();

            return View("Edit", res);
        }

        // --------------------------------------------------------------------
        //  EDIT RESOURCE (POST)
        // --------------------------------------------------------------------

        /// <summary>
        /// Updates an existing resource after input validation.
        /// Logs the edit event.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Edit(int id, string name, string type, int quantity)
        {
            var res = await _repo.GetById(id);
            if (res == null) return NotFound();

            // Validate updated data
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

            await _logging.AddAsync(
                $"Admin edited resource (id={res.Id}, name={res.Name})");

            return RedirectToAction("Index");
        }

        // --------------------------------------------------------------------
        //  DELETE RESOURCE
        // --------------------------------------------------------------------

        /// <summary>
        /// Deletes a resource entry from the system.
        /// Logs the deletion for audit tracking.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _repo.GetById(id);
            if (res == null) return NotFound();

            _repo.Delete(res);
            await _repo.Save();

            await _logging.AddAsync(
                $"Admin deleted resource (id={res.Id}, name={res.Name})");

            return RedirectToAction("Index");
        }
    }
}
