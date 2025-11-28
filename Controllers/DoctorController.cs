/**************************************************************************
 * File: DoctorController.cs
 * Authors:
 *     • Omrahn Faqiri  — Implemented full doctor-side functionality:
 *                         - Appointment listing
 *                         - Uploading test results
 *                         - Confirming & rejecting appointments
 *                         - Viewing specific test result details
 *
 *     • Maryam Elhamidi — Added LoggingService integration, 
 *                         documentation, cleanup, and UX alignment.
 *
 * Description:
 *     Provides all doctor-facing functionality in MediScope:
 *         - Doctor dashboard showing upcoming & past appointments
 *         - Uploading laboratory/diagnostic test results
 *         - Viewing detailed test result pages
 *         - Accept/Reject workflow for pending appointments
 *
 *     This controller enforces:
 *         - Role-based access ("Doctor" only)
 *         - Linking doctors to Identity users
 *         - Proper audit logging for every sensitive operation
 *
 * Last Modified: Nov 26, 2025
 **************************************************************************/

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediScope.Models;
using MediScope.Services;
using System.Security.Claims;

namespace MediScope.Controllers
{
    /// <summary>
    /// Handles all doctor-specific operations such as managing appointments,
    /// uploading test results, and processing upcoming bookings.
    /// </summary>
    [Authorize(Roles = "Doctor")]
    public class DoctorController : Controller
    {
        private readonly MediScopeContext _context;
        private readonly LoggingService _logging;

        /// <summary>
        /// Constructor injecting EF DbContext and LoggingService.
        /// </summary>
        public DoctorController(MediScopeContext context, LoggingService logging)
        {
            _context = context;
            _logging = logging;
        }

        // --------------------------------------------------------------------
        //  DOCTOR DASHBOARD (LIST APPOINTMENTS)
        // --------------------------------------------------------------------

        /// <summary>
        /// Loads the doctor's dashboard, showing all patient appointments 
        /// associated with the logged-in doctor. Includes patient test results.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Find doctor linked to logged-in user
            var doctor = await _context.Doctors.FirstOrDefaultAsync(x => x.UserId == userId);
            if (doctor == null)
                return RedirectToAction("Login", "Account");

            // Retrieve all appointments belonging to this doctor
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .ThenInclude(p => p.TestResults)
                .Where(a => a.DoctorId == doctor.Id)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return View("Index", appointments);
        }

        // --------------------------------------------------------------------
        //  UPLOAD TEST RESULTS (GET)
        // --------------------------------------------------------------------

        /// <summary>
        /// Returns the form for uploading a test result for the selected appointment.
        /// </summary>
        [HttpGet]
        public IActionResult UploadTestResult(int appointmentId, int patientId)
        {
            ViewBag.AppointmentId = appointmentId;
            ViewBag.PatientId = patientId;

            return View("UploadTestResult");
        }

        // --------------------------------------------------------------------
        //  UPLOAD TEST RESULTS (POST)
        // --------------------------------------------------------------------

        /// <summary>
        /// Handles submission of a new test result by the doctor.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UploadTestResult(int appointmentId, int patientId, string testName, string result)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var doctor = await _context.Doctors.FirstOrDefaultAsync(x => x.UserId == userId);
            if (doctor == null)
                return RedirectToAction("Index");

            // Create a new test result entry
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

            await _logging.AddAsync(
                $"Doctor (id={doctor.Id}) uploaded test result (id={test.Id}) for patient (id={patientId})");

            return RedirectToAction("Index");
        }

        // --------------------------------------------------------------------
        //  VIEW TEST RESULT DETAILS
        // --------------------------------------------------------------------

        /// <summary>
        /// Displays detailed information of a specific test result.
        /// </summary>
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

        // --------------------------------------------------------------------
        //  ACCEPT APPOINTMENT
        // --------------------------------------------------------------------

        /// <summary>
        /// Allows a doctor to accept a pending appointment request.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AcceptAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
                return RedirectToAction("Index");

            appointment.Status = "Confirmed";
            await _context.SaveChangesAsync();

            await _logging.AddAsync($"Doctor confirmed appointment (id={id})");

            return RedirectToAction("Index");
        }

        // --------------------------------------------------------------------
        //  REJECT APPOINTMENT
        // --------------------------------------------------------------------

        /// <summary>
        /// Allows a doctor to reject an appointment request.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> RejectAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
                return RedirectToAction("Index");

            appointment.Status = "Rejected";
            await _context.SaveChangesAsync();

            await _logging.AddAsync($"Doctor rejected appointment (id={id})");

            return RedirectToAction("Index");
        }
    }
}
