using MediScope.Models;
using Microsoft.EntityFrameworkCore;

namespace MediScope.Repositories
{
    public class AdminRepository
    {
        private readonly MediScopeContext _context;

        public AdminRepository(MediScopeContext context)
        {
            _context = context;
        }

        // Get all admins
        public async Task<IEnumerable<Administrator>> GetAllAdminsAsync()
        {
            return await _context.Administrators
                .Include(a => a.User)
                .ToListAsync();
        }

        // Get overview stats
        public async Task<(int doctors, int patients, double avgRating)> GetSystemStatsAsync()
        {
            var doctors = await _context.Doctors.CountAsync();
            var patients = await _context.Patients.CountAsync();
            var rating = await _context.Feedbacks.AnyAsync()
                ? await _context.Feedbacks.AverageAsync(f => f.Rating)
                : 0;

            return (doctors, patients, rating);
        }

        // Get feedback with nav props
        public async Task<IEnumerable<Feedback>> GetFeedbackAsync()
        {
            return await _context.Feedbacks
                .Include(f => f.Doctor)
                .Include(f => f.Patient)
                .ToListAsync();
        }
    }
}