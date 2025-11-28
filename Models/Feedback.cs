/**************************************************************************
 * File: Feedback.cs
 * Author: Omrahn Faqiri
 *
 * Description:
 *      Represents feedback given by patients to doctors.

 *
 * Last Modified: Nov 27, 2025
 **************************************************************************/
namespace MediScope.Models;

public class Feedback
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public string Message { get; set; } = "";
    public int Rating { get; set; } // 1–5
    public Patient? Patient { get; set; }
    public Doctor? Doctor { get; set; }


}