using MediScope.Identity;
using MediScope.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MediScope.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider _serviceProvider, MediScopeContext context)
    {
        var userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // ######### Ensure Roles #########
        string[] roles = { "Admin", "Doctor", "Patient" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // ######### Seed Admin #########
        var adminUser = await userManager.FindByNameAsync("admin");
        if (adminUser == null)
        {
            var newAdmin = new ApplicationUser { UserName = "admin", Email = "admin@mediscope.com" };
            var result = await userManager.CreateAsync(newAdmin, "Admin@123!");
            if (!result.Succeeded)
                throw new Exception("Failed to create admin user: " + string.Join(", ", result.Errors.Select(e => e.Description)));

            adminUser = await userManager.FindByNameAsync("admin");

            context.Administrators.Add(new Administrator
            {
                UserId = adminUser.Id,
                Name = "John Doe"
            });
            await context.SaveChangesAsync();
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        // ######### Seed Doctors #########
        var doctorsData = new[]
        {
            new { Username = "doctor1", Name = "Dr. Sarah Johnson", Specialty = "Family Medicine" },
            new { Username = "doctor2", Name = "Dr. Mark Patel", Specialty = "Cardiology" }
        };

        foreach (var d in doctorsData)
        {
            var docUser = await userManager.FindByNameAsync(d.Username);
            if (docUser == null)
            {
                var newDocUser = new ApplicationUser { UserName = d.Username, Email = d.Username + "@mediscope.com" };
                var result = await userManager.CreateAsync(newDocUser, "Doc@123!");
                if (!result.Succeeded)
                    throw new Exception("Failed to create doctor user: " + string.Join(", ", result.Errors.Select(e => e.Description)));

                docUser = await userManager.FindByNameAsync(d.Username);

                context.Doctors.Add(new Doctor
                {
                    UserId = docUser.Id,
                    Name = d.Name,
                    Specialty = d.Specialty
                });
                await context.SaveChangesAsync();

                await userManager.AddToRoleAsync(docUser, "Doctor");
            }
        }

        // ######### Seed Patients #########
        var patientsData = new[]
        {
            new { Username = "patient1", Name = "John Doe", Age = 30, Gender = "Male", Email = "john@example.com" },
            new { Username = "patient2", Name = "Emily Chen", Age = 24, Gender = "Female", Email = "emily@example.com" }
        };

        foreach (var p in patientsData)
        {
            var patUser = await userManager.FindByNameAsync(p.Username);
            if (patUser == null)
            {
                var newPatUser = new ApplicationUser { UserName = p.Username, Email = p.Username + "@mediscope.com" };
                var result = await userManager.CreateAsync(newPatUser, "Pat@123!");
                if (!result.Succeeded)
                    throw new Exception("Failed to create patient user: " + string.Join(", ", result.Errors.Select(e => e.Description)));

                patUser = await userManager.FindByNameAsync(p.Username);

                context.Patients.Add(new Patient
                {
                    UserId = patUser.Id,
                    Name = p.Name,
                    Age = p.Age,
                    Gender = p.Gender,
                    Email = p.Email
                });
                await context.SaveChangesAsync();

                await userManager.AddToRoleAsync(patUser, "Patient");
            }
        }

        // ######### Seed Feedback #########
        var doctorProfiles = context.Doctors.ToList();
        var patientProfiles = context.Patients.ToList();

        if (!context.Feedbacks.Any() && doctorProfiles.Count >= 2 && patientProfiles.Count >= 2)
        {
            context.Feedbacks.AddRange(
                new Feedback
                {
                    DoctorId = doctorProfiles[0].Id,
                    PatientId = patientProfiles[0].Id,
                    Rating = 5,
                    Message = "Great service!"
                },
                new Feedback
                {
                    DoctorId = doctorProfiles[1].Id,
                    PatientId = patientProfiles[1].Id,
                    Rating = 1,
                    Message = "Rude staff."
                }
            );
        }
        await context.SaveChangesAsync();

        // ######### Seed Analytics #########
        if (!context.AnalyticsRecords.Any())
        {
            context.AnalyticsRecords.AddRange(new List<AnalyticsRecord>
            {
                new AnalyticsRecord { MetricName = "AverageWaitTime", Value = 12.5, RecordedAt = DateTime.Now.AddDays(-10) },
                new AnalyticsRecord { MetricName = "AverageWaitTime", Value = 10.2, RecordedAt = DateTime.Now.AddDays(-9) },
                new AnalyticsRecord { MetricName = "AverageWaitTime", Value = 8.8, RecordedAt = DateTime.Now.AddDays(-8) },
                new AnalyticsRecord { MetricName = "AverageFeedbackRating", Value = 4.5, RecordedAt = DateTime.Now.AddDays(-7) },
                new AnalyticsRecord { MetricName = "AverageFeedbackRating", Value = 4.8, RecordedAt = DateTime.Now.AddDays(-6) },
                new AnalyticsRecord { MetricName = "AppointmentsScheduled", Value = 25, RecordedAt = DateTime.Now.AddDays(-5) },
                new AnalyticsRecord { MetricName = "AppointmentsScheduled", Value = 30, RecordedAt = DateTime.Now.AddDays(-4) },
                new AnalyticsRecord { MetricName = "CancelledAppointments", Value = 2, RecordedAt = DateTime.Now.AddDays(-3) },
                new AnalyticsRecord { MetricName = "CancelledAppointments", Value = 1, RecordedAt = DateTime.Now.AddDays(-2) },
                new AnalyticsRecord { MetricName = "NewPatients", Value = 5, RecordedAt = DateTime.Now.AddDays(-1) }
            });
            await context.SaveChangesAsync();
        }

        // ######### (Seed Appointments) #########
        if (!context.Appointments.Any())
        {
            context.Appointments.AddRange(
                new Appointment
                {
                    DoctorId = doctorProfiles[0].Id,
                    PatientId = patientProfiles[0].Id,
                    Date = DateTime.Now.AddDays(1),
                    Status = "Scheduled"
                },
                new Appointment
                {
                    DoctorId = doctorProfiles[1].Id,
                    PatientId = patientProfiles[1].Id,
                    Date = DateTime.Now.AddDays(2),
                    Status = "Scheduled"
                }
            );
        }

        await context.SaveChangesAsync();

        
        // ######### Ensure TestResults table exists #########
        if (!context.TestResults.Any())
        {
            await context.SaveChangesAsync();
        }
    }
}
