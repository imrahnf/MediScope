using MediScope.Identity;

namespace MediScope.Models;

public class Administrator
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    // FK to Identity
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
}