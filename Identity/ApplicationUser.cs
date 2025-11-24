using MediScope.Models;
using Microsoft.AspNetCore.Identity;
namespace MediScope.Identity;


public class ApplicationUser : IdentityUser
{
    // Navigation properties
    public Administrator? Administrator { get; set; }
    public Doctor? Doctor { get; set; }
    public Patient? Patient { get; set; }
}