namespace MediScope.Models;

public class Patient
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Age { get; set; }
    public string Gender { get; set; } = "";
    public string Email { get; set; } = "";
    
    public List<Appointment>? Appointments { get; set; }
}