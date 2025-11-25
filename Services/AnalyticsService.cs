using Microsoft.EntityFrameworkCore;
using MediScope.Models;

namespace MediScope.Services
{
    public class AnalyticsService
    {
        private readonly MediScopeContext _context;

        public AnalyticsService(MediScopeContext context)
        {
            _context = context;
        }

        public async Task<double> GetAverageFeedbackRatingAsync()
        {
            if (!await _context.Feedbacks.AnyAsync()) return 0.0;
            return await _context.Feedbacks.AverageAsync(f => (double)f.Rating);
        }

        public async Task<IEnumerable<AnalyticsRecord>> GetRecentAnalyticsAsync(int take = 30)
        {
            return await _context.AnalyticsRecords
                .OrderByDescending(a => a.RecordedAt)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetScheduledAppointmentCountAsync(DateTime? since = null)
        {
            var q = _context.Appointments.AsQueryable();
            if (since.HasValue) q = q.Where(a => a.Date >= since.Value);
            return await q.CountAsync(a => a.Status == "Scheduled");
        }
    }
}