using Microsoft.EntityFrameworkCore;
using MediScope.Models;

namespace MediScope.Services
{
    public class FeedbackService
    {
        private readonly MediScopeContext _context;

        public FeedbackService(MediScopeContext context)
        {
            _context = context;
        }

        public async Task SubmitFeedbackAsync(Feedback feedback)
        {
            await _context.Feedbacks.AddAsync(feedback);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Feedback>> GetAllFeedbacksAsync() =>
            await _context.Feedbacks.Include(f => f.Patient).Include(f => f.Doctor).ToListAsync();

        public async Task<IEnumerable<Feedback>> GetFeedbacksForDoctorAsync(int doctorId) =>
            await _context.Feedbacks.Where(f => f.DoctorId == doctorId).ToListAsync();

        public async Task<bool> HasPatientSubmittedFeedbackAsync(int patientId, int doctorId)
        {
            return await _context.Feedbacks.AnyAsync(f => f.PatientId == patientId && f.DoctorId == doctorId);
        }
    }
}