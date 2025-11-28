/**************************************************************************
 * File: DoctorRepository.cs
 * Author: Maryam Elhamidi
 *
 * Description:
 *     Custom repository for doctor-specific data access.
 *     Includes Identity-linked doctor lookup and doctor–patient relations.
 *
 * Last Modified: Nov 26, 2025
 **************************************************************************/

using MediScope.Models;
using Microsoft.EntityFrameworkCore;

namespace MediScope.Repositories
{
    /// <summary>
    /// Provides doctor-related queries beyond basic CRUD.
    /// </summary>
    public class DoctorRepository : Repository<Doctor>
    {
        private readonly MediScopeContext _context;

        public DoctorRepository(MediScopeContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a doctor based on their ASP.NET Identity UserId.
        /// Used for matching logged-in doctors to their profile.
        /// </summary>
        public async Task<Doctor?> GetByUserIdAsync(string userId)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.UserId == userId);
        }

        /// <summary>
        /// Returns all unique patients who have had appointments with a doctor.
        /// </summary>
        public async Task<IEnumerable<Patient>> GetPatientsForDoctorAsync(int doctorId)
        {
            var patientIds = await _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .Select(a => a.PatientId)
                .Distinct()
                .ToListAsync();

            return await _context.Patients
                .Where(p => patientIds.Contains(p.Id))
                .ToListAsync();
        }

        /// <summary>
        /// Returns total number of appointments a doctor has.
        /// </summary>
        public async Task<int> GetAppointmentCountAsync(int doctorId)
        {
            return await _context.Appointments
                .CountAsync(a => a.DoctorId == doctorId);
        }
    }
}
