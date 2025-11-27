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
        private readonly LoggingService _logging;

        public PatientController(AppointmentService appointmentService, MediScopeContext context, FeedbackService feedbackService, LoggingService logging)
        {
            _appointmentService = appointmentService;
            _context = context;
            _feedbackService = feedbackService;
            _logging = logging;
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
            // Provide list of doctors for the feedback form
            var doctors = await _context.Doctors.ToListAsync();
            ViewData["Doctors"] = doctors;

            // show any success message from previous submission
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Feedback(int doctorId, int rating, string message)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null) return RedirectToAction("Index");
            
            var doctor = await _context.Doctors.FindAsync(doctorId);
            if (doctor == null)
            {
                ModelState.AddModelError("doctorId", "Selected doctor does not exist.");
            }

            if (rating < 1 || rating > 5)
            {
                ModelState.AddModelError("rating", "Rating must be between 1 and 5.");
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                ModelState.AddModelError("message", "Please enter your feedback message.");
            }
            else if (message.Length > 1000)
            {
                ModelState.AddModelError("message", "Message must be 1000 characters or fewer.");
            }

            // prevent more than one feedback per patient->doctor
            if (doctor != null)
            {
                var already = await _feedbackService.HasPatientSubmittedFeedbackAsync(patient.Id, doctorId);
                if (already)
                {
                    ModelState.AddModelError(string.Empty, "You have already submitted feedback for this doctor.");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewData["Doctors"] = await _context.Doctors.ToListAsync();
                return View();
            }

            var feedback = new Feedback
            {
                DoctorId = doctorId,
                PatientId = patient.Id,
                Rating = rating,
                Message = message
            };

            await _feedbackService.SubmitFeedbackAsync(feedback);

            // Log the action
            await _logging.AddAsync($"Patient (id={patient.Id}) gave doctor (id={doctorId}) feedback");

            TempData["Message"] = "Thank you. Your feedback was submitted.";
            return RedirectToAction("Feedback");
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

        [HttpGet]
        public async Task<IActionResult> DownloadTestResult(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null) return RedirectToAction("Login", "Account");

            var result = await _context.TestResults
                .Include(tr => tr.Doctor)
                .FirstOrDefaultAsync(tr => tr.Id == id && tr.PatientId == patient.Id);

            if (result == null)
            {
                TempData["Message"] = "Test result not found or does not belong to you.";
                return RedirectToAction("TestResults");
            }

            // Generate text file content
            var content = $"TEST RESULT\n";
            content += "================================================================================\n\n";
            content += $"Patient Name:     {patient.Name}\n";
            content += $"Test Name:        {result.TestName}\n";
            content += $"Date Performed:   {result.DatePerformed:MMMM dd, yyyy}\n";
            content += $"Doctor:           {result.Doctor?.Name ?? "N/A"}\n\n";
            content += "RESULT:\n";
            content += "--------------------------------------------------------------------------------\n";
            content += $"{result.Result}\n";
            content += "\n================================================================================\n";
            content += $"Generated on: {DateTime.Now:MMMM dd, yyyy HH:mm}\n";

            var bytes = System.Text.Encoding.UTF8.GetBytes(content);
            var fileName = $"TestResult_{result.TestName.Replace(" ", "_")}_{result.DatePerformed:yyyyMMdd}.txt";
            
            // Log the download
            await _logging.AddAsync($"Patient (id={patient.Id}) downloaded test result (id={result.Id})");

            return File(bytes, "text/plain", fileName);
         }
     }
 }
