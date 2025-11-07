using Microsoft.EntityFrameworkCore;

namespace MediScope.Models;

public class MediScopeContext : DbContext
{
    public MediScopeContext(DbContextOptions<MediScopeContext> options) :  base(options) {}
    
    public DbSet<User> Users { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<Administrator> Administrators { get; set; }
    public DbSet<AnalyticsRecord> AnalyticsRecords { get; set; }
    public DbSet<TestResult> TestResults { get; set; }
    
}