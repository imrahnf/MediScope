/**************************************************************************
 * File: Appointment.cs
 * Author: Omrahn Faqiri
 *
 * Description:
 *     Represents a medical appointment between a patient and a doctor.
 *     Appointments have a date, status, and link to both patient and doctor.
 *
 * Last Modified: Nov 27, 2025
 **************************************************************************/
namespace MediScope.Models;

public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = "Scheduled"; // default=Scheduled, others: Completed, Cancelled

    public Patient? Patient { get; set; }
    public Doctor? Doctor { get; set; }
}