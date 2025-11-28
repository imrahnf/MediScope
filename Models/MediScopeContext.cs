/**************************************************************************
 * File: MediScopeContext.cs
 * Author: Omrahn Faqiri
 *
 * Description:
 *     The Entity Framework Core database context for the MediScope application.
 *     Inherits from IdentityDbContext to integrate ASP.NET Core Identity for user management.
 *     Defines DbSet properties for each entity in the application, enabling CRUD operations
 *     on Patients, Doctors, Appointments, Feedback, and more.
 *
 * Last Modified: Nov 27, 2025
 **************************************************************************/
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
    public DbSet<Resource> Resources { get; set; }
    public DbSet<Log> Logs { get; set; }
}