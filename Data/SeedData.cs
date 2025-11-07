using MediScope.Models;
using Microsoft.EntityFrameworkCore;

namespace MediScope.Data;

public static class SeedData
{
    public static void Initialize(MediScopeContext context)
    {
        // ######### (seeded data generated with ChatGPT) #########

        // admins
        var administrator1 = context.Administrators.FirstOrDefault(a => a.Name == "omrahn");
        if (administrator1 == null)
        {
            administrator1 = new Administrator { Name = "omrahn", Email = "omrahn@gmail.com" };
            context.Administrators.Add(administrator1);
        }

        var administrator2 = context.Administrators.FirstOrDefault(a => a.Name == "maryam");
        if (administrator2 == null)
        {
            administrator2 = new Administrator { Name = "maryam", Email = "maryam@gmail.com" };
            context.Administrators.Add(administrator2);
        }

        context.SaveChanges();
        
        // initial doctor data 
        var doctor1 = context.Doctors.FirstOrDefault(d => d.Name == "Dr. Sarah Patel");
        if (doctor1 == null)
        {
            doctor1 = new Doctor { Name = "Dr. Sarah Patel", Specialty = "Cardiology" };
            context.Doctors.Add(doctor1);
        }

        var doctor2 = context.Doctors.FirstOrDefault(d => d.Name == "Dr. James Wong");
        if (doctor2 == null)
        {
            doctor2 = new Doctor { Name = "Dr. James Wong", Specialty = "Neurology" };
            context.Doctors.Add(doctor2);
        }

        context.SaveChanges();

        // seed initial patient data
        var patient1 = context.Patients.FirstOrDefault(p => p.Name == "John Doe");
        if (patient1 == null)
        {
            patient1 = new Patient { Name = "John Doe", Age = 34, Gender = "Male", Email = "john@gmail.com" };
            context.Patients.Add(patient1);
        }

        var patient2 = context.Patients.FirstOrDefault(p => p.Name == "Jane Smith");
        if (patient2 == null)
        {
            patient2 = new Patient { Name = "Jane Smith", Age = 28, Gender = "Female", Email = "jane@gmail.com" };
            context.Patients.Add(patient2);
        }

        context.SaveChanges();

        // users
        if (!context.Users.Any(u => u.Username == "admin"))
            context.Users.Add(new User { Username = "admin", Password = "admin", Role = "Admin" });

        if (!context.Users.Any(u => u.Username == "doc1"))
            context.Users.Add(new User { Username = "doc1", Password = "123", Role = "Doctor", DoctorId = doctor1.Id });

        if (!context.Users.Any(u => u.Username == "doc2"))
            context.Users.Add(new User { Username = "doc2", Password = "123", Role = "Doctor", DoctorId = doctor2.Id });

        if (!context.Users.Any(u => u.Username == "pat1"))
            context.Users.Add(new User { Username = "pat1", Password = "123", Role = "Patient", PatientId = patient1.Id });

        if (!context.Users.Any(u => u.Username == "pat2"))
            context.Users.Add(new User { Username = "pat2", Password = "123", Role = "Patient", PatientId = patient2.Id });

        context.SaveChanges();

        // apopintmnts
        if (!context.Appointments.Any(a => a.PatientId == patient1.Id && a.DoctorId == doctor1.Id))
            context.Appointments.Add(new Appointment { PatientId = patient1.Id, DoctorId = doctor1.Id, Date = DateTime.Now.AddDays(1), Status = "Scheduled" });

        if (!context.Appointments.Any(a => a.PatientId == patient2.Id && a.DoctorId == doctor2.Id))
            context.Appointments.Add(new Appointment { PatientId = patient2.Id, DoctorId = doctor2.Id, Date = DateTime.Now.AddDays(2), Status = "Scheduled" });

        context.SaveChanges();

        // feedback
        if (!context.Feedbacks.Any(f => f.PatientId == patient1.Id && f.DoctorId == doctor1.Id))
            context.Feedbacks.Add(new Feedback { PatientId = patient1.Id, DoctorId = doctor1.Id, Message = "Great service!", Rating = 5 });

        if (!context.Feedbacks.Any(f => f.PatientId == patient2.Id && f.DoctorId == doctor2.Id))
            context.Feedbacks.Add(new Feedback { PatientId = patient2.Id, DoctorId = doctor2.Id, Message = "Friendly staff.", Rating = 4 });

        context.SaveChanges();
        
        // no initial data required, table exists for future seeding
        if (!context.TestResults.Any())
        {
            context.SaveChanges();
        }
        
        // no initial data required, table exists for future metrics/charts
        if (!context.AnalyticsRecords.Any())
        {
            
            var analyticsSeed = new List<AnalyticsRecord>
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
            };

            context.AnalyticsRecords.AddRange(analyticsSeed);
            context.SaveChanges();
            
        }
    }
}
