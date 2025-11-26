using MediScope.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MediScope.Models;

public class MediScopeContext : IdentityDbContext<ApplicationUser>
{
    public MediScopeContext(DbContextOptions<MediScopeContext> options) :  base(options) {}
    
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Administrator> Administrators { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<AnalyticsRecord> AnalyticsRecords { get; set; }
    public DbSet<TestResult> TestResults { get; set; }
    public DbSet<Department> Departments { get; set; }

    
}