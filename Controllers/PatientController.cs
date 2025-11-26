using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediScope.Services;
using System.Security.Claims;
using MediScope.Models;
using Microsoft.EntityFrameworkCore;

namespace MediScope.Controllers
{

    [Authorize(Roles = "Patient")]
    public class PatientController : Controller
    {
        private readonly AppointmentService _appointmentService;
        private readonly MediScopeContext _context;
        private readonly FeedbackService _feedbackService;

        public PatientController(AppointmentService appointmentService, MediScopeContext context, FeedbackService feedbackService)
        {
            _appointmentService = appointmentService;
            _context = context;
            _feedbackService = feedbackService;
        }

        public async Task<IActionResult> Index()
        {
            // get current patient's Patient model via linked ApplicationUser
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null) return RedirectToAction("Login", "Account");

            var appts = await _appointmentService.GetAppointmentsForPatientAsync(patient.Id);
            return View(appts);
        }

        [HttpGet]
        public async Task<IActionResult> Feedback()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Feedback(int doctorId, int rating, string message)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null) return RedirectToAction("Index");

            var feedback = new Feedback
            {
                DoctorId = doctorId,
                PatientId = patient.Id,
                Rating = rating,
                Message = message
            };

            await _feedbackService.SubmitFeedbackAsync(feedback);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> TestResults()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null) return RedirectToAction("Login", "Account");

            var results = await _context.TestResults
                .Where(tr => tr.PatientId == patient.Id)
                .Include(tr => tr.Doctor)
                .OrderByDescending(tr => tr.DatePerformed)
                .ToListAsync();

            return View(results);
        }

        [HttpGet]
        public async Task<IActionResult> TestResult(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null) return RedirectToAction("Login", "Account");

            var result = await _context.TestResults
                .Include(tr => tr.Doctor)
                .Include(tr => tr.Appointment)
                .FirstOrDefaultAsync(tr => tr.Id == id && tr.PatientId == patient.Id);

            if (result == null)
            {
                TempData["Message"] = "Test result not found or does not belong to you.";
                return RedirectToAction("TestResults");
            }

            return View("TestResult", result);
        }
    }
}