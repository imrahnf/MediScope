using MediScope.Identity;

namespace MediScope.Models;


public class Doctor
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Specialty { get; set; } = "";

    // FK to Identity
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
}