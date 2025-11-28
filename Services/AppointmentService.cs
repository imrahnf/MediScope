/**************************************************************************
 * File: AppointmentService.cs
 * Author: Omrahn (Core logic), with integration scripting by Maryam Elhamidi
 *
 * Description:
 *     Contains all business logic related to appointments: booking,
 *     cancellation, availability detection, and retrieval.
 *     Applied by PatientController and DoctorController.
 *
 * Last Modified: Nov 26, 2025
 **************************************************************************/

using MediScope.Models;
using Microsoft.EntityFrameworkCore;

namespace MediScope.Services
{
    /// <summary>
    /// Handles business rules for appointment scheduling and workflow.
    /// </summary>
    public class AppointmentService
    {
        private readonly MediScopeContext _context;

        public AppointmentService(MediScopeContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns available appointment slots for a doctor on a given date.
        /// Assumes clinic hours 9–16 with 1-hour increments.
        /// </summary>
        public async Task<IEnumerable<DateTime>> GetAvailableSlotsAsync(int doctorId, DateTime date)
        {
            var slots = Enumerable.Range(9, 8).Select(h =>
                new DateTime(date.Year, date.Month, date.Day, h, 0, 0));

            var existing = await _context.Appointments
                .Where(a => a.DoctorId == doctorId && a.Date.Date == date.Date && a.Status == "Scheduled")
                .Select(a => a.Date)
                .ToListAsync();

            return slots.Where(s => !existing.Any(e => e == s));
        }

        /// <summary>
        /// Books an appointment after validating time collisions and date rules.
        /// </summary>
        public async Task<(bool Success, string? Error)> BookAppointmentAsync(int patientId, int doctorId, DateTime dateTime)
        {
            if (dateTime < DateTime.Now) 
                return (false, "Cannot book in the past.");

            bool conflict = await _context.Appointments.AnyAsync(a =>
                a.DoctorId == doctorId && a.Date == dateTime && a.Status == "Scheduled");

            if (conflict) 
                return (false, "Doctor not available at that time.");

            var appointment = new Appointment
            {
                PatientId = patientId,
                DoctorId = doctorId,
                Date = dateTime,
                Status = "Scheduled"
            };

            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();

            return (true, null);
        }

        /// <summary>
        /// Cancels a scheduled appointment (patient-initiated only).
        /// </summary>
        public async Task<bool> CancelAppointmentAsync(int appointmentId, int requestingUserPatientId)
        {
            var appt = await _context.Appointments.FindAsync(appointmentId);
            if (appt == null || appt.PatientId != requestingUserPatientId)
                return false;

            appt.Status = "Cancelled";
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Marks an appointment as completed (doctor-initiated).
        /// </summary>
        public async Task<bool> CompleteAppointmentAsync(int appointmentId, int doctorUserId)
        {
            var appt = await _context.Appointments.FindAsync(appointmentId);
            if (appt == null)
                return false;

            appt.Status = "Completed";
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Returns all appointments for a patient, including doctor details.
        /// </summary>
        public async Task<IEnumerable<Appointment>> GetAppointmentsForPatientAsync(int patientId)
        {
            return await _context.Appointments
                .Where(a => a.PatientId == patientId)
                .Include(a => a.Doctor)
                .OrderByDescending(a => a.Date)
                .ToListAsync();
        }

        /// <summary>
        /// Returns all upcoming appointments for a doctor.
        /// </summary>
        public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsForDoctorAsync(int doctorId)
        {
            return await _context.Appointments
                .Where(a => a.DoctorId == doctorId && a.Date >= DateTime.Now && a.Status == "Scheduled")
                .Include(a => a.Patient)
                .OrderBy(a => a.Date)
                .ToListAsync();
        }
    }
}
