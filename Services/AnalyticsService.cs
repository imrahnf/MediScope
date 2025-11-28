/**************************************************************************
 * File: AnalyticsService.cs
 * Author: Maryam Elhamidi
 *
 * Description:
 *     Provides statistical and analytical data used throughout the Admin
 *     Dashboard. Includes average ratings, weekly trends, appointment
 *     summaries, and sentiment breakdowns for visualization via Chart.js.
 *
 * Last Modified: Nov 26, 2025
 **************************************************************************/

using MediScope.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace MediScope.Services
{
    /// <summary>
    /// Handles aggregation and analytical computations over system data
    /// (appointments, feedback, metrics). Used by Admin dashboards.
    /// </summary>
    public class AnalyticsService
    {
        private readonly MediScopeContext _context;

        public AnalyticsService(MediScopeContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Computes the average doctor rating across all feedback entries.
        /// Returns 0 if no feedback exists.
        /// </summary>
        public async Task<double> GetAverageFeedbackRatingAsync()
        {
            if (!await _context.Feedbacks.AnyAsync())
                return 0;

            return await _context.Feedbacks.AverageAsync(f => f.Rating);
        }

        /// <summary>
        /// Returns the most recent N analytics records for displaying trends.
        /// </summary>
        public async Task<List<AnalyticsRecord>> GetRecentAnalyticsAsync(int days)
        {
            return await _context.AnalyticsRecords
                .OrderByDescending(a => a.RecordedAt)
                .Take(days)
                .ToListAsync();
        }

        /// <summary>
        /// Returns weekly appointment counts formatted as date ranges (Mon–Sun).
        /// Used for weekly trend charts.
        /// </summary>
        public async Task<List<object>> GetWeeklyAppointmentCountsAsync()
        {
            return await Task.Run(() =>
                _context.Appointments
                    .AsEnumerable()
                    .GroupBy(a =>
                    {
                        var week = ISOWeek.GetWeekOfYear(a.Date);
                        var year = a.Date.Year;

                        DateTime firstDay = ISOWeek.ToDateTime(year, week, DayOfWeek.Monday);
                        DateTime lastDay = firstDay.AddDays(6);

                        return new { Year = year, Week = week, Start = firstDay, End = lastDay };
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

        /// <summary>
        /// Returns average rating per doctor for comparison charts.
        /// </summary>
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

        /// <summary>
        /// Splits feedback into positive/neutral/negative categories
        /// for pie chart visualizations.
        /// </summary>
        public async Task<object> GetFeedbackSentimentAsync()
        {
            int pos = await _context.Feedbacks.CountAsync(f => f.Rating >= 4);
            int neu = await _context.Feedbacks.CountAsync(f => f.Rating == 3);
            int neg = await _context.Feedbacks.CountAsync(f => f.Rating <= 2);

            return new { positive = pos, neutral = neu, negative = neg };
        }
    }
}
