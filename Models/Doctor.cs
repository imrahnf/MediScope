/**************************************************************************
 * File: Doctor.cs
 * Author: Omrahn Faqiri
 *
 * Description:
 *     Represents a medical doctor in the MediScope system.
 *     Doctors have specialties and can be linked to user accounts.
 *
 * Last Modified: Nov 27, 2025
 **************************************************************************/
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
    // DEPARTMENT
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }

}