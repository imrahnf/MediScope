using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediScope.Models;
using MediScope.Services;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace MediScope.Controllers
{
    [Authorize(Roles = "Patient")]
    public class AppointmentController : Controller
    {
        private readonly MediScopeContext _context;
        private readonly AppointmentService _appointmentService;
        private readonly LoggingService _logging;

        public AppointmentController(MediScopeContext context, AppointmentService appointmentService, LoggingService logging)
        {
            _context = context;
            _appointmentService = appointmentService;
            _logging = logging;
        }

        [HttpGet]
        public async Task<IActionResult> Book()
        {
            var doctors = await _context.Doctors.ToListAsync();
            return View(doctors); // view should render doctor list and slot selection
        }

        [HttpPost]
        public async Task<IActionResult> Book(int doctorId, DateTime date, int hour)
        {
            var dateTime = new DateTime(date.Year, date.Month, date.Day, hour, 0, 0);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null) return RedirectToAction("Login", "Account");

            var (success, error) = await _appointmentService.BookAppointmentAsync(patient.Id, doctorId, dateTime);
            if (!success)
            {
                TempData["Error"] = error;
                return RedirectToAction("Book");
            }

            // Log the booking
            await _logging.AddAsync($"Patient (id={patient.Id}) booked appointment with doctor (id={doctorId}) on {dateTime:O}");

            return RedirectToAction("Index", "Patient");
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int appointmentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null) return Forbid();

            var ok = await _appointmentService.CancelAppointmentAsync(appointmentId, patient.Id);
            if (!ok) return BadRequest();

            // Log the cancellation
            await _logging.AddAsync($"Patient (id={patient.Id}) canceled appointment (id={appointmentId})");

            return RedirectToAction("Index", "Patient");
        }
    }
}
