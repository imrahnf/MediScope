using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediScope.Models;
using MediScope.Services;

namespace MediScope.Controllers;

    [Route("api/admin/analytics")]
    [ApiController]
    public class AdminAnalyticsApiController : ControllerBase
    {
        private readonly MediScopeContext _context;
        private readonly AnalyticsService _analytics;

        public AdminAnalyticsApiController(MediScopeContext context, AnalyticsService analytics)
        {
            _context = context;
            _analytics = analytics;
        }

        [HttpGet("appointments")]
        public async Task<IActionResult> GetAppointmentsTrend()
        {
            var data = await _context.AnalyticsRecords
                .Where(a => a.MetricName == "AppointmentsScheduled")
                .OrderBy(a => a.RecordedAt)
                .Select(a => new { date = a.RecordedAt, value = a.Value })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("ratings")]
        public async Task<IActionResult> GetRatings()
        {
            var avg = await _analytics.GetAverageFeedbackRatingAsync();
            return Ok(new { average = avg });
        }

        [HttpGet("statusBreakdown")]
        public async Task<IActionResult> StatusBreakdown()
        {
            var scheduled = await _context.Appointments.CountAsync(a => a.Status == "Scheduled");
            var completed = await _context.Appointments.CountAsync(a => a.Status == "Completed");
            var cancelled = await _context.Appointments.CountAsync(a => a.Status == "Cancelled");

            return Ok(new
            {
                scheduled,
                completed,
                cancelled
            });
        }
        [HttpGet("weeklyAppointments")]
        public async Task<IActionResult> GetWeeklyAppointments()
        {
            var data = await _analytics.GetWeeklyAppointmentCountsAsync();
            return Ok(data);
        }

        [HttpGet("doctorRatings")]
        public async Task<IActionResult> GetDoctorRatings()
        {
            var data = await _analytics.GetDoctorRatingsAsync();
            return Ok(data);
        }

        [HttpGet("feedbackSentiment")]
        public async Task<IActionResult> GetFeedbackSentiment()
        {
            var data = await _analytics.GetFeedbackSentimentAsync();
            return Ok(data);
        }
    }
    