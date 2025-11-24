using MediScope.Identity;

namespace MediScope.Models;

public class Patient
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Age { get; set; }
    public string Gender { get; set; } = "";
    public string Email { get; set; } = "";

    // FK to Identity
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
}