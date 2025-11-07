namespace MediScope.Models;

public class Feedback
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public string Message { get; set; } = "";
    public int Rating { get; set; } // 1–5

}