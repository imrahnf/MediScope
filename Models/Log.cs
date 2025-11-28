/**************************************************************************
 * File: Log.cs
 * Author: Omrahn Faqiri
 *
 * Description:
 *     Represents a log entry for auditing purposes.
 *     Each log contains a description and a timestamp.
 *
 * Last Modified: Nov 27, 2025
 **************************************************************************/
namespace MediScope.Models;

public class Log
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
