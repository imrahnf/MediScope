using MediScope.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;


namespace MediScope.Services
{
    public class AnalyticsService
    {
        private readonly MediScopeContext _context;

        public AnalyticsService(MediScopeContext context)
        {
            _context = context;
        }

        // Average doctor rating
        public async Task<double> GetAverageFeedbackRatingAsync()
        {
            if (!await _context.Feedbacks.AnyAsync())
                return 0;

            return await _context.Feedbacks.AverageAsync(f => f.Rating);
        }

        // Get last X days of appointment data
        public async Task<List<AnalyticsRecord>> GetRecentAnalyticsAsync(int days)
        {
            return await _context.AnalyticsRecords
                .OrderByDescending(a => a.RecordedAt)
                .Take(days)
                .ToListAsync();
        }

        // Weekly appointment totals
        public async Task<List<object>> GetWeeklyAppointmentCountsAsync()
        {
            return await Task.Run(() =>
                _context.Appointments
                    .AsEnumerable()
                    .GroupBy(a =>
                    {
                        var week = ISOWeek.GetWeekOfYear(a.Date);
                        var year = a.Date.Year;

                        // Calculate start of ISO week
                        DateTime firstDay = ISOWeek.ToDateTime(year, week, DayOfWeek.Monday);
                        DateTime lastDay = firstDay.AddDays(6);

                        return new
                        {
                            Year = year,
                            Week = week,
                            Start = firstDay,
                            End = lastDay
                        };
                    })
                    .Select(g => new
                    {
                        weekLabel = $"{g.Key.Start:MMM dd} – {g.Key.End:MMM dd}",
                        count = g.Count()
                    })
                    .OrderBy(x => x.weekLabel)
                    .Cast<object>()
                    .ToList()
            );
        }

        // Doctor rating averages
        public async Task<List<object>> GetDoctorRatingsAsync()
        {
            var ratings = await _context.Feedbacks
                .GroupBy(f => f.DoctorId)
                .Select(g => new
                {
                    Doctor = _context.Doctors.First(d => d.Id == g.Key).Name,
                    AvgRating = g.Average(f => f.Rating)
                })
                .ToListAsync();

            return ratings.Cast<object>().ToList();
        }


        // Feedback sentiment (positive/neutral/negative)
        public async Task<object> GetFeedbackSentimentAsync()
        {
            int pos = await _context.Feedbacks.CountAsync(f => f.Rating >= 4);
            int neu = await _context.Feedbacks.CountAsync(f => f.Rating == 3);
            int neg = await _context.Feedbacks.CountAsync(f => f.Rating <= 2);

            return new { positive = pos, neutral = neu, negative = neg };
        }
    }
}
