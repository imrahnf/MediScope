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

            // Show all appointments for this doctor, ordered by date (newest first)
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .ThenInclude(p => p.TestResults)
                .Where(a => a.DoctorId == doctor.Id)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return View("Index", appointments);
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
                AppointmentId = appointmentId,
                TestName = testName,
                Result = result,
                DatePerformed = DateTime.Now
            };

            _context.TestResults.Add(test);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> TestResultPage(int id)
        {
            var testResult = await _context.TestResults
                .Include(t => t.Patient)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (testResult == null)
                return RedirectToAction("Index");

            return View("TestResultPage", testResult);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            
            if (appointment == null)
                return RedirectToAction("Index");

            appointment.Status = "Confirmed";
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RejectAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            
            if (appointment == null)
                return RedirectToAction("Index");

            appointment.Status = "Rejected";
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
