namespace MediScope.Models;

public class TestResult
{
    public int Id { get; set; }
    public string TestName { get; set; } = null!;
    public string Result { get; set; } = null!;
    public DateTime DatePerformed { get; set; } = DateTime.Now;

    // Relationships
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
    
    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;

    // link to the appointment
    public int? AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }
}