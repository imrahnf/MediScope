namespace MediScope.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string Role { get; set; } = ""; // "Patient", "Doctor", or "Admin"

    // to help reference the specific doctor/patient
    public int? DoctorId { get; set; }
    public int? PatientId { get; set; }
}