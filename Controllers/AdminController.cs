using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediScope.Services;
using MediScope.Models;
using Microsoft.EntityFrameworkCore;

namespace MediScope.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly MediScopeContext _context;
        private readonly AnalyticsService _analytics;

        public AdminController(MediScopeContext context, AnalyticsService analytics)
        {
            _context = context;
            _analytics = analytics;
        }

        public async Task<IActionResult> Index()
        {
            var avgRating = await _analytics.GetAverageFeedbackRatingAsync();
            var recentAnalytics = await _analytics.GetRecentAnalyticsAsync(20);
            ViewBag.AvgRating = avgRating;
            return View(recentAnalytics);
        }

        public async Task<IActionResult> Doctors()
        {
            var doctors = await _context.Doctors.ToListAsync();
            return View(doctors);
        }

        [HttpGet]
        public IActionResult CreateDoctor() => View();

        [HttpPost]
        public async Task<IActionResult> CreateDoctor(string name, string specialty, string? userId)
        {
            var doctor = new Doctor { Name = name, Specialty = specialty, UserId = userId };
            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();
            return RedirectToAction("Doctors");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var d = await _context.Doctors.FindAsync(id);
            if (d == null) return NotFound();
            _context.Doctors.Remove(d);
            await _context.SaveChangesAsync();
            return RedirectToAction("Doctors");
        }

        public async Task<IActionResult> Feedback()
        {
            var feedbacks = await _context.Feedbacks.Include(f => f.Doctor).Include(f => f.Patient).ToListAsync();
            return View(feedbacks);
        }
    }
}
