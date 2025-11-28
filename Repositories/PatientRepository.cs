/**************************************************************************
 * File: PatientRepository.cs
 * Author: Maryam Elhamidi
 *
 * Description:
 *     Custom repository for patient-specific data access.
 *     Includes appointment and test result lookups for patient dashboards.
 *
 * Last Modified: Nov 26, 2025
 **************************************************************************/

using MediScope.Models;
using Microsoft.EntityFrameworkCore;

namespace MediScope.Repositories
{
    /// <summary>
    /// Provides patient-specific queries beyond basic CRUD.
    /// </summary>
    public class PatientRepository : Repository<Patient>
    {
        private readonly MediScopeContext _context;

        public PatientRepository(MediScopeContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns all appointments for a specific patient, including doctor details.
        /// </summary>
        public async Task<IEnumerable<Appointment>> GetAppointmentsAsync(int patientId)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patientId)
                .ToListAsync();
        }

        /// <summary>
        /// Returns all test results linked to a specific patient.
        /// </summary>
        public async Task<IEnumerable<TestResult>> GetTestResultsAsync(int patientId)
        {
            return await _context.TestResults
                .Where(t => t.PatientId == patientId)
                .ToListAsync();
        }
    }
}