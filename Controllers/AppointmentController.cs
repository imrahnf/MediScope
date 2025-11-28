/**************************************************************************
 * File: AppointmentController.cs
 * Authors:
 *     • Omrahn Faqiri  — Appointment booking, cancellation logic,
 *                         and integration with AppointmentService.
 *     • Maryam Elhamidi — Logging integration, cleanup, documentation,
 *                         and overall system alignment.
 *
 * Description:
 *     Handles all patient-side appointment functionality including:
 *         - Viewing available doctors
 *         - Selecting appointment dates & times
 *         - Booking appointments using AppointmentService
 *         - Canceling scheduled appointments
 *         - Logging all actions for audit purposes
 *
 *     This controller ensures:
 *         - Only authenticated patients may access appointment features
 *         - Domain rules are enforced via AppointmentService
 *         - All actions are tracked using LoggingService
 *
 * Last Modified: Nov 26, 2025
 **************************************************************************/

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediScope.Models;
using MediScope.Services;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace MediScope.Controllers
{    /// <summary>
    /// Handles appointment booking and cancellation features for Patients.
    /// Requires the "Patient" role.
    /// </summary>
    [Authorize(Roles = "Patient")]
    public class AppointmentController : Controller
    {
        private readonly MediScopeContext _context;
        private readonly AppointmentService _appointmentService;
        private readonly LoggingService _logging;

        
        /// <summary>
        /// Omrahn - Constructor injecting database context, appointment logic service, 
        /// and logging service for audit tracking.
        /// </summary>
        public AppointmentController(MediScopeContext context, AppointmentService appointmentService, LoggingService logging)
        {
            _context = context;
            _appointmentService = appointmentService;
            _logging = logging;
        }

        // --------------------------------------------------------------------
        //  BOOKING APPOINTMENT (GET)
        // --------------------------------------------------------------------

        /// <summary>
        /// Maryam - Displays a list of all doctors so the patient can choose who to book with.
        /// A corresponding view will allow selecting the date & time.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Book()
        {
            var doctors = await _context.Doctors.ToListAsync();
            return View(doctors); // view should render doctor list and slot selection
        }
        // --------------------------------------------------------------------
        //  BOOKING APPOINTMENT (POST) - maryam 
        // --------------------------------------------------------------------

        /// <summary>
        /// Handles the POST booking action. A patient selects a doctor, date, 
        /// and hour. AppointmentService enforces all domain rules.
        /// </summary>
        /// <param name="doctorId">Doctor being booked.</param>
        /// <param name="date">Selected appointment date.</param>
        /// <param name="hour">Selected appointment hour (0–23).</param>
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
        // --------------------------------------------------------------------
        //  CANCEL APPOINTMENT
        // --------------------------------------------------------------------

        /// <summary>
        /// Allows a patient to cancel one of their appointments.
        /// AppointmentService ensures only the owner may cancel.
        /// </summary>
        /// <param name="appointmentId">Appointment to cancel.</param>
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
