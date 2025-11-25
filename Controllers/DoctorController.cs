using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediScope.Models;
using System.Security.Claims;

namespace MediScope.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class DoctorController : Controller
    {
        private readonly MediScopeContext _context;

        public DoctorController(MediScopeContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(x => x.UserId == userId);

            if (doctor == null)
                return RedirectToAction("Login", "Account");

            var today = DateTime.Today;

            var todaysAppointments = await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctor.Id && a.Date.Date == today)
                .ToListAsync();

            return View("Index", todaysAppointments);
        }

        [HttpGet]
        public IActionResult UploadTestResult(int appointmentId, int patientId)
        {
            ViewBag.AppointmentId = appointmentId;
            ViewBag.PatientId = patientId;
            return View("UploadTestResult");
        }

        [HttpPost]
        public async Task<IActionResult> UploadTestResult(int appointmentId, int patientId, string testName, string result)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(x => x.UserId == userId);

            if (doctor == null)
                return RedirectToAction("Index");

            var test = new TestResult
            {
                DoctorId = doctor.Id,
                PatientId = patientId,
                TestName = testName,
                Result = result,
                DatePerformed = DateTime.Now
            };

            _context.TestResults.Add(test);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
