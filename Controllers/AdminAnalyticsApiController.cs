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
        // TOTAL DOCTORS
        [HttpGet("totalDoctors")]
        public async Task<IActionResult> GetTotalDoctors()
        {
            var count = await _context.Doctors.CountAsync();
            return Ok(new { count });
        }

        // TOTAL PATIENTS 
        [HttpGet("totalPatients")]
        public async Task<IActionResult> GetTotalPatients()
        {
            var count = await _context.Patients.CountAsync();
            return Ok(new { count });
        }

        // TOTAL APPOINTMENTS
        [HttpGet("totalAppointments")]
        public async Task<IActionResult> GetTotalAppointments()
        {
            var count = await _context.Appointments.CountAsync();
            return Ok(new { count });
        }

        // WEEKLY APPOINTMENTS SUMMARY
        [HttpGet("weeklySummary")]
        public async Task<IActionResult> GetWeeklySummary()
        {
            var since = DateTime.UtcNow.AddDays(-7);

            var count = await _context.Appointments
                .Where(a => a.Date >= since)
                .CountAsync();

            return Ok(new { count });
        }
    }
    