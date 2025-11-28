/**************************************************************************
 * File: FeedbackService.cs
 * Author: Omrahn (structure) with expansion by Maryam Elhamidi
 *
 * Description:
 *     Provides core operations for patient feedback creation and retrieval.
 *     Used by Admin dashboard and Doctor insights.
 *
 * Last Modified: Nov 26, 2025
 **************************************************************************/

using Microsoft.EntityFrameworkCore;
using MediScope.Models;

namespace MediScope.Services
{
    /// <summary>
    /// Handles the creation and retrieval of feedback records.
    /// Includes helper methods to avoid duplicate submissions.
    /// </summary>
    public class FeedbackService
    {
        private readonly MediScopeContext _context;

        public FeedbackService(MediScopeContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Saves a new feedback record to the database.
        /// </summary>
        public async Task SubmitFeedbackAsync(Feedback feedback)
        {
            await _context.Feedbacks.AddAsync(feedback);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Returns all feedback including patient and doctor navigation properties.
        /// </summary>
        public async Task<IEnumerable<Feedback>> GetAllFeedbacksAsync() =>
            await _context.Feedbacks.Include(f => f.Patient).Include(f => f.Doctor).ToListAsync();

        /// <summary>
        /// Returns feedback for a specific doctor.
        /// </summary>
        public async Task<IEnumerable<Feedback>> GetFeedbacksForDoctorAsync(int doctorId) =>
            await _context.Feedbacks.Where(f => f.DoctorId == doctorId).ToListAsync();

        /// <summary>
        /// Checks whether a patient has already reviewed a doctor.
        /// Prevents duplicate ratings.
        /// </summary>
        public async Task<bool> HasPatientSubmittedFeedbackAsync(int patientId, int doctorId)
        {
            return await _context.Feedbacks.AnyAsync(f => f.PatientId == patientId && f.DoctorId == doctorId);
        }
    }
}
