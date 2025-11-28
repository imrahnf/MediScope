/**************************************************************************
 * File: AdminAnalyticsApiController.cs
 * Author: Maryam Elhamidi
 * Description:
 *     Provides Web API endpoints that supply JSON analytics data for the 
 *     Admin Dashboard. This includes appointment trends, doctor ratings, 
 *     patient feedback sentiment, weekly appointment counts, and general 
 *     system statistics. All endpoints support Chart.js visualizations.
 * 
 *     This controller is part of the MediScope administrative analytics module.
 *     It communicates with AnalyticsService, LoggingService, and EF Core.
 * 
 * Last Modified: Nov 26, 2025
 **************************************************************************/
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediScope.Models;
using MediScope.Services;

namespace MediScope.Controllers;
/// <summary>
/// API controller responsible for returning admin analytics data in JSON format.
/// Used by the Admin Dashboard for generating charts and key KPIs.
/// </summary>
    [Route("api/admin/analytics")]
    [ApiController]
    public class AdminAnalyticsApiController : ControllerBase
    {
        private readonly MediScopeContext _context;
        private readonly AnalyticsService _analytics;
        private readonly LoggingService _logging;
        
        /// <summary>
        /// Constructor injecting the application's DbContext, analytics logic service, 
        /// and logging service for system auditing.
        /// </summary>
        public AdminAnalyticsApiController(MediScopeContext context, AnalyticsService analytics, LoggingService logging)
        {
            _context = context;
            _analytics = analytics;
            _logging = logging;
        }

        /// <summary>
        /// Returns the historical daily appointment counts recorded in the AnalyticsRecords table.
        /// Used for the "Appointment Trends" line chart.
        /// </summary>
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
        /// <summary>
        /// Returns the average doctor rating calculated across all patient feedback entries.
        /// Used in the rating bar chart.
        /// </summary>
        [HttpGet("ratings")]
        public async Task<IActionResult> GetRatings()
        {
            await _logging.AddAsync("AdminAnalytics: GetRatings called");
            var avg = await _analytics.GetAverageFeedbackRatingAsync();
            return Ok(new { average = avg });
        }
        /// <summary>
        /// Returns a breakdown of appointments by status (Scheduled, Completed, Cancelled).
        /// Used for the status pie chart.
        /// </summary>
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
        /// <summary>
        /// Returns weekly appointment totals formatted for the "Weekly Appointments" chart.
        /// Grouping logic handled inside AnalyticsService.
        /// </summary>
        [HttpGet("weeklyAppointments")]
        public async Task<IActionResult> GetWeeklyAppointments()
        {
            await _logging.AddAsync("AdminAnalytics: GetWeeklyAppointments called");
            var data = await _analytics.GetWeeklyAppointmentCountsAsync();
            return Ok(data);
        }
        /// <summary>
        /// Returns the average rating for each doctor.
        /// Used in the Doctor Ratings bar chart.
        /// </summary>
        [HttpGet("doctorRatings")]
        public async Task<IActionResult> GetDoctorRatings()
        {
            await _logging.AddAsync("AdminAnalytics: GetDoctorRatings called");
            var data = await _analytics.GetDoctorRatingsAsync();
            return Ok(data);
        }
        /// <summary>
        /// Returns counts of positive, neutral, and negative feedback based on rating categories.
        /// Used in the Feedback Sentiment pie chart.
        /// </summary>
        [HttpGet("feedbackSentiment")]
        public async Task<IActionResult> GetFeedbackSentiment()
        {
            await _logging.AddAsync("AdminAnalytics: GetFeedbackSentiment called");
            var data = await _analytics.GetFeedbackSentimentAsync();
            return Ok(data);
        }
        // TOTAL DOCTORS
        /// <summary>
        /// Returns total number of doctors in the system.
        /// Displayed in the admin dashboard KPI row.
        /// </summary>
        [HttpGet("totalDoctors")]
        public async Task<IActionResult> GetTotalDoctors()
        {
            await _logging.AddAsync("AdminAnalytics: GetTotalDoctors called");
            var count = await _context.Doctors.CountAsync();
            return Ok(new { count });
        }

        // TOTAL PATIENTS 
        /// <summary>
        /// Returns total number of patients in the system.
        /// Displayed in the admin dashboard KPI row.
        /// </summary>
        [HttpGet("totalPatients")]
        public async Task<IActionResult> GetTotalPatients()
        {
            await _logging.AddAsync("AdminAnalytics: GetTotalPatients called");
            var count = await _context.Patients.CountAsync();
            return Ok(new { count });
        }

        // TOTAL APPOINTMENTS
        /// <summary>
        /// Returns total number of appointments stored.
        /// Displayed in the admin dashboard KPI row.
        /// </summary>
        [HttpGet("totalAppointments")]
        public async Task<IActionResult> GetTotalAppointments()
        {
            await _logging.AddAsync("AdminAnalytics: GetTotalAppointments called");
            var count = await _context.Appointments.CountAsync();
            return Ok(new { count });
        }

        // WEEKLY APPOINTMENTS SUMMARY
        /// <summary>
        /// Returns the number of appointments created within the past 7 days.
        /// Used for the "Weekly Appointment Summary" card.
        /// </summary>
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
