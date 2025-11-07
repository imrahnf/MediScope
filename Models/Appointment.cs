namespace MediScope.Models;

public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = "Scheduled"; // default=Scheduled, others: Completed, Cancelled

    public Patient? Patient { get; set; }
    public Doctor? Doctor { get; set; }
}