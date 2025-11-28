/**************************************************************************
 * File: AdminRepository.cs
 * Author: Maryam Elhamidi
 *
 * Description:
 *     Custom repository for Admin-specific queries.
 *     Provides system-level data aggregation used by Admin dashboards,
 *     including statistics, admin listings, and feedback retrieval.
 *
 * Last Modified: Nov 26, 2025
 **************************************************************************/

using MediScope.Models;
using Microsoft.EntityFrameworkCore;

namespace MediScope.Repositories
{
    /// <summary>
    /// Repository containing specialized queries used by the Admin module.
    /// </summary>
    public class AdminRepository
    {
        private readonly MediScopeContext _context;

        public AdminRepository(MediScopeContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns all admin accounts with Identity user navigation properties.
        /// </summary>
        public async Task<IEnumerable<Administrator>> GetAllAdminsAsync()
        {
            return await _context.Administrators
                .Include(a => a.User)
                .ToListAsync();
        }

        /// <summary>
        /// Returns high-level system statistics used in dashboard KPIs.
        /// Includes total doctors, total patients, and average rating.
        /// </summary>
        public async Task<(int doctors, int patients, double avgRating)> GetSystemStatsAsync()
        {
            var doctors = await _context.Doctors.CountAsync();
            var patients = await _context.Patients.CountAsync();

            var rating = await _context.Feedbacks.AnyAsync()
                ? await _context.Feedbacks.AverageAsync(f => f.Rating)
                : 0;

            return (doctors, patients, rating);
        }

        /// <summary>
        /// Returns all feedback records including doctor and patient nav props.
        /// </summary>
        public async Task<IEnumerable<Feedback>> GetFeedbackAsync()
        {
            return await _context.Feedbacks
                .Include(f => f.Doctor)
                .Include(f => f.Patient)
                .ToListAsync();
        }
    }
}
