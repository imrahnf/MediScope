/**************************************************************************
 * File: Administrator.cs
 * Author: Omrahn Faqiri
 *
 * Description:
 *     Represents an administrator user in the MediScope system.
 *     Administrators have elevated privileges to manage system settings,
 *     departments, and user accounts.
 *
 * Last Modified: Nov 27, 2025
 **************************************************************************/
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