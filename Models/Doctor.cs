namespace MediScope.Models;

public class Doctor
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Specialty { get; set; } = "";

    // Navigation
    public List<Appointment>? Appointments { get; set; }
    public List<Feedback>? Feedbacks { get; set; }
}