using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediScope.Models;
using MediScope.Services;
using MediScope.Models.ViewModels;

namespace MediScope.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly MediScopeContext _context;
        private readonly AnalyticsService _analytics;
        private readonly ValidationService _validator;
        private readonly LoggingService _logging = null!;

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

        // LANDING PAGE
        public IActionResult Index()
        {
            return View("Index");
        }

        // DASHBOARD
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

        // DOCTOR MANAGEMENT
        public async Task<IActionResult> Doctors()
        {
            var doctors = await _context.Doctors.ToListAsync();
            return View("Doctors", doctors);
        }

        [HttpGet]
        public async Task<IActionResult> CreateDoctor()
        {
            ViewBag.Departments = await _context.Departments.ToListAsync();
            return View("CreateDoctor");
        }


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

        [HttpGet]
        public async Task<IActionResult> EditDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return NotFound();

            ViewBag.Departments = await _context.Departments.ToListAsync();
            return View("EditDoctor", doctor);
        }

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
        // FEEDBACK REVIEW
        public async Task<IActionResult> Feedback()
        {
            var feedback = await _context.Feedbacks
                .Include(f => f.Doctor)
                .Include(f => f.Patient)
                .ToListAsync();

            await _logging.AddAsync($"Admin accessed feedback review (count={feedback.Count})");
            return View("Feedback", feedback);
        }

        // PLACEHOLDER: RESOURCE MANAGEMENT
        public IActionResult Resources()
        {
            return View("Resources");
        }

        // PLACEHOLDER: DEPARTMENT MANAGEMENT
        public IActionResult Departments()
        {
            // Will be implemented when we create Department model
            return View("Departments");
        }
    }
}
