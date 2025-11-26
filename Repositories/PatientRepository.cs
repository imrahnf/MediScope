using MediScope.Models;
using Microsoft.EntityFrameworkCore;

namespace MediScope.Repositories;

public class PatientRepository : Repository<Patient>
{
    private readonly MediScopeContext _context;

    public PatientRepository(MediScopeContext context) : base(context)
    {
        _context = context;
    }

    // All appointments for a patient
    public async Task<IEnumerable<Appointment>> GetAppointmentsAsync(int patientId)
    {
        return await _context.Appointments
            .Include(a => a.Doctor)
            .Where(a => a.PatientId == patientId)
            .ToListAsync();
    }

    // All test results for a patient
    public async Task<IEnumerable<TestResult>> GetTestResultsAsync(int patientId)
    {
        return await _context.TestResults
            .Where(t => t.PatientId == patientId)
            .ToListAsync();
    }
}

