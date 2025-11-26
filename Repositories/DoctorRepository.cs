using MediScope.Models;
using Microsoft.EntityFrameworkCore;

namespace MediScope.Repositories
{
    public class DoctorRepository : Repository<Doctor>
    {
        private readonly MediScopeContext _context;

        public DoctorRepository(MediScopeContext context) : base(context)
        {
            _context = context;
        }

        // Get doctor by UserId (Identity link)
        public async Task<Doctor?> GetByUserIdAsync(string userId)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.UserId == userId);
        }

        // Get doctor + all patients they saw
        public async Task<IEnumerable<Patient>> GetPatientsForDoctorAsync(int doctorId)
        {
            var patientIds = await _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .Select(a => a.PatientId)
                .Distinct()
                .ToListAsync();

            return await _context.Patients
                .Where(p => patientIds.Contains(p.Id))
                .ToListAsync();
        }

        // Get doctor appointment count
        public async Task<int> GetAppointmentCountAsync(int doctorId)
        {
            return await _context.Appointments
                .CountAsync(a => a.DoctorId == doctorId);
        }
    }
}