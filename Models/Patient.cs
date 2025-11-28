/**************************************************************************
 * File: Patient.cs
 * Author: Omrahn Faqiri
 *
 * Description:
 *     This file defines the Patient model for the MediScope application.
 *     The Patient class includes properties for storing patient information
 *     such as Id, Name, Age, Gender, and Email. It also establishes a foreign key
 *     relationship with the ApplicationUser class from the Identity namespace,
 *     allowing each patient to be associated with a user account. Additionally,
 *     the Patient class contains a navigation property for related TestResult entities.
 *     This structure facilitates the management of patient data within MediScope.
 *
 * Last Modified: Nov 27, 2025
 **************************************************************************/
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

    // Navigation properties
    public List<TestResult> TestResults { get; set; } = new();
}