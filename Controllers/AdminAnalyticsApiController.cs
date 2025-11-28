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
        private readonly LoggingService _logging;

        public AdminAnalyticsApiController(MediScopeContext context, AnalyticsService analytics, LoggingService logging)
        {
            _context = context;
            _analytics = analytics;
            _logging = logging;
        }

        [HttpGet("appointments")]
        public async Task<IActionResult> GetAppointmentsTrend()
        {
            await _logging.AddAsync("AdminAnalytics: GetAppointmentsTrend called");
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
            await _logging.AddAsync("AdminAnalytics: GetRatings called");
            var avg = await _analytics.GetAverageFeedbackRatingAsync();
            return Ok(new { average = avg });
        }

        [HttpGet("statusBreakdown")]
        public async Task<IActionResult> StatusBreakdown()
        {
            await _logging.AddAsync("AdminAnalytics: StatusBreakdown called");
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
            await _logging.AddAsync("AdminAnalytics: GetWeeklyAppointments called");
            var data = await _analytics.GetWeeklyAppointmentCountsAsync();
            return Ok(data);
        }

        [HttpGet("doctorRatings")]
        public async Task<IActionResult> GetDoctorRatings()
        {
            await _logging.AddAsync("AdminAnalytics: GetDoctorRatings called");
            var data = await _analytics.GetDoctorRatingsAsync();
            return Ok(data);
        }

        [HttpGet("feedbackSentiment")]
        public async Task<IActionResult> GetFeedbackSentiment()
        {
            await _logging.AddAsync("AdminAnalytics: GetFeedbackSentiment called");
            var data = await _analytics.GetFeedbackSentimentAsync();
            return Ok(data);
        }
        // TOTAL DOCTORS
        [HttpGet("totalDoctors")]
        public async Task<IActionResult> GetTotalDoctors()
        {
            await _logging.AddAsync("AdminAnalytics: GetTotalDoctors called");
            var count = await _context.Doctors.CountAsync();
            return Ok(new { count });
        }

        // TOTAL PATIENTS 
        [HttpGet("totalPatients")]
        public async Task<IActionResult> GetTotalPatients()
        {
            await _logging.AddAsync("AdminAnalytics: GetTotalPatients called");
            var count = await _context.Patients.CountAsync();
            return Ok(new { count });
        }

        // TOTAL APPOINTMENTS
        [HttpGet("totalAppointments")]
        public async Task<IActionResult> GetTotalAppointments()
        {
            await _logging.AddAsync("AdminAnalytics: GetTotalAppointments called");
            var count = await _context.Appointments.CountAsync();
            return Ok(new { count });
        }

        // WEEKLY APPOINTMENTS SUMMARY
        [HttpGet("weeklySummary")]
        public async Task<IActionResult> GetWeeklySummary()
        {
            await _logging.AddAsync("AdminAnalytics: GetWeeklySummary called");
            var since = DateTime.UtcNow.AddDays(-7);

            var count = await _context.Appointments
                .Where(a => a.Date >= since)
                .CountAsync();

            return Ok(new { count });
        }
    }
