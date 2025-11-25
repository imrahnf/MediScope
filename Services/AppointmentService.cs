using MediScope.Models;
using Microsoft.EntityFrameworkCore;

namespace MediScope.Services
{
    public class AppointmentService
    {
        private readonly MediScopeContext _context;

        public AppointmentService(MediScopeContext context)
        {
            _context = context;
        }

        // Return available slots for a doctor on a given date
        public async Task<IEnumerable<DateTime>> GetAvailableSlotsAsync(int doctorId, DateTime date)
        {
            // assume clinic hours 9:00 - 16:00, 1-hour slots
            var slots = Enumerable.Range(9, 8)
                .Select(h => new DateTime(date.Year, date.Month, date.Day, h, 0, 0));

            var existing = await _context.Appointments
                .Where(a => a.DoctorId == doctorId && a.Date.Date == date.Date && a.Status == "Scheduled")
                .Select(a => a.Date)
                .ToListAsync();

            return slots.Where(s => !existing.Any(e => e == s));
        }

        public async Task<(bool Success, string? Error)> BookAppointmentAsync(int patientId, int doctorId, DateTime dateTime)
        {
            // Basic validation
            if (dateTime < DateTime.Now) return (false, "Cannot book in the past.");
            
            // prevent double booking same doctor same time
            var conflict = await _context.Appointments.AnyAsync(a => a.DoctorId == doctorId
                                                                     && a.Date == dateTime
                                                                     && a.Status == "Scheduled");
            if (conflict) return (false, "Doctor not available at that time.");

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

        public async Task<bool> CancelAppointmentAsync(int appointmentId, int requestingUserPatientId)
        {
            var appt = await _context.Appointments.FindAsync(appointmentId);
            if (appt == null) return false;
            if (appt.PatientId != requestingUserPatientId) return false;

            appt.Status = "Cancelled";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CompleteAppointmentAsync(int appointmentId, int doctorUserId)
        {
            var appt = await _context.Appointments.FindAsync(appointmentId);
            if (appt == null) return false;

            // In a real app validate doctorUserId maps to the appointment's doctor.
            appt.Status = "Completed";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsForPatientAsync(int patientId)
        {
            return await _context.Appointments
                .Where(a => a.PatientId == patientId)
                .Include(a => a.Doctor)
                .OrderByDescending(a => a.Date)
                .ToListAsync();
        }

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
