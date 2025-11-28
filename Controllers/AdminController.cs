/**************************************************************************
 * File: AdminController.cs
 * Author: Maryam Elhamidi & Omrahn
 * Description:
 *     Handles all administrative UI logic for the MediScope system including:
 *         - Dashboard analytics
 *         - Doctor management (CRUD)
 *         - Patient feedback review
 *         - Department & resource placeholders
 *         - Admin system logs
 * 
 *     This controller works closely with:
 *         - AnalyticsService (charts + KPIs)
 *         - ValidationService (business rule validation)
 *         - LoggingService (admin audit trail)
 *         - MediScopeContext (database access)
 *
 *     All actions require an Admin role.
 *
 * Last Modified: Nov 26, 2025
 **************************************************************************/

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediScope.Models;
using MediScope.Services;
using MediScope.Models.ViewModels;

namespace MediScope.Controllers
{
    /// <summary>
    /// Maryam - MVC controller that manages all admin-side features such as dashboards,
    /// doctor management, feedback moderation, and system logs.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly MediScopeContext _context;
        private readonly AnalyticsService _analytics;
        private readonly ValidationService _validator;
        private readonly LoggingService _logging;

        /// <summary>
        /// Maryam - Injects database, analytics, validation, and logging services.
        /// </summary>
        public AdminController(
            MediScopeContext context,
            AnalyticsService analytics,
            ValidationService validator,
            LoggingService logging)
        {
            _context = context;
            _analytics = analytics;
            _validator = validator;
            _logging = logging;
        }

        // --------------------------------------------------------------------
        //  LANDING PAGE
        // --------------------------------------------------------------------

        /// <summary>
        /// Omrahn - Displays the admin landing page.
        /// </summary>
        public IActionResult Index()
        {
            return View("Index");
        }

        // --------------------------------------------------------------------
        //  ADMIN DASHBOARD
        // --------------------------------------------------------------------

        /// <summary>
        /// Maryam - Loads the Admin Dashboard with total counts, recent analytics,
        /// and average feedback rating. All chart requests are handled through AJAX.
        /// </summary>
        public async Task<IActionResult> Dashboard()
        {
            var model = new AdminDashboardViewModel
            {
                TotalDoctors = await _context.Doctors.CountAsync(),
                TotalPatients = await _context.Patients.CountAsync(),
                AverageRating = await _analytics.GetAverageFeedbackRatingAsync(),
                RecentRecords = await _analytics.GetRecentAnalyticsAsync(7)
            };

            return View("Dashboard", model);
        }

        // --------------------------------------------------------------------
        //  DOCTOR MANAGEMENT
        // --------------------------------------------------------------------

        /// <summary>
        /// Maryam - Displays a list of all doctors in the system.
        /// </summary>
        public async Task<IActionResult> Doctors()
        {
            var doctors = await _context.Doctors.ToListAsync();
            return View("Doctors", doctors);
        }

        /// <summary>
        /// Maryam - Shows the "Create Doctor" form.
        /// Departments are loaded for dropdown selection.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CreateDoctor()
        {
            ViewBag.Departments = await _context.Departments.ToListAsync();
            return View("CreateDoctor");
        }

        /// <summary>
        /// Maryam - Creates a new doctor after validating the input.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateDoctor(string name, string specialty, int departmentId)
        {
            var validation = _validator.ValidateDoctorCreation(name, specialty);

            if (!validation.Success)
            {
                ViewBag.Error = validation.Message;
                ViewBag.Departments = await _context.Departments.ToListAsync();
                return View("CreateDoctor");
            }

            var doctor = new Doctor
            {
                Name = name,
                Specialty = specialty,
                DepartmentId = departmentId
            };

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            await _logging.AddAsync($"Admin created doctor (name={name}, id={doctor.Id})");

            return RedirectToAction("Doctors");
        }

        /// <summary>
        /// Maryam - Loads the edit page for a specific doctor.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> EditDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return NotFound();

            ViewBag.Departments = await _context.Departments.ToListAsync();
            return View("EditDoctor", doctor);
        }

        /// <summary>
        /// Maryam - Updates an existing doctor after validation.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> EditDoctor(int id, string name, string specialty, int departmentId)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return NotFound();

            var validation = _validator.ValidateDoctorCreation(name, specialty);

            if (!validation.Success)
            {
                ViewBag.Error = validation.Message;
                ViewBag.Departments = await _context.Departments.ToListAsync();
                return View("EditDoctor", doctor);
            }

            doctor.Name = name;
            doctor.Specialty = specialty;
            doctor.DepartmentId = departmentId;

            await _context.SaveChangesAsync();
            await _logging.AddAsync($"Admin edited doctor (id={id}, name={name})");

            return RedirectToAction("Doctors");
        }

        /// <summary>
        /// Maryam - Deletes a doctor from the system.
        /// Logs the deletion for accountability.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return NotFound();

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();

            await _logging.AddAsync($"Admin deleted doctor (id={id}, name={doctor.Name})");

            return RedirectToAction("Doctors");
        }

        // --------------------------------------------------------------------
        //  PATIENT FEEDBACK MANAGEMENT
        // --------------------------------------------------------------------

        /// <summary>
        /// Maryam - Retrieves all feedback entries for admin review, including doctor & patient info.
        /// </summary>
        public async Task<IActionResult> Feedback()
        {
            var feedback = await _context.Feedbacks
                .Include(f => f.Doctor)
                .Include(f => f.Patient)
                .ToListAsync();

            await _logging.AddAsync($"Admin accessed feedback review (count={feedback.Count})");

            return View("Feedback", feedback);
        }

        // --------------------------------------------------------------------
        //  PLACEHOLDERS FOR FUTURE MODULES
        // --------------------------------------------------------------------

        /// <summary>
        /// Maryam -Placeholder page for the resource management module.
        /// </summary>
        public IActionResult Resources()
        {
            return View("Resources");
        }

        /// <summary>
        /// Maryam - Placeholder page for the department management module.
        /// </summary>
        public IActionResult Departments()
        {
            return View("Departments");
        }

        // --------------------------------------------------------------------
        //  SYSTEM LOGS
        // --------------------------------------------------------------------

        /// <summary>
        /// Omrahn - Shows the application logs with optional search and filtering.
        /// </summary>
        public async Task<IActionResult> Logs(string? q = null, int take = 200)
        {
            var query = _context.Logs.AsQueryable();

            // optional search filter
            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(l => l.Description.ToLower().Contains(q.ToLower()));
                ViewData["Query"] = q;
            }

            var logs = await query
                .OrderByDescending(l => l.CreatedAt)
                .Take(take)
                .ToListAsync();

            ViewData["Take"] = take;

            return View("Logs", logs);
        }

        /// <summary>
        /// Omrahn - Clears all logs and records the action for audit.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearLogs()
        {
            var admin = User?.Identity?.Name ?? "Admin";

            _context.Logs.RemoveRange(_context.Logs);
            await _context.SaveChangesAsync();

            await _logging.AddAsync($"Admin '{admin}' cleared all logs");

            return RedirectToAction("Logs");
        }
    }
}
