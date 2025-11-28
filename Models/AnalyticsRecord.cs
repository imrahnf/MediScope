/**************************************************************************
 * File: AnalyticsRecord.cs
 * Author: Omrahn Faqiri
 *
 * Description:
 *      Represents a record of analytics data collected within the MediScope system.
 *
 * Last Modified: Nov 27, 2025
 **************************************************************************/

namespace MediScope.Models;

public class AnalyticsRecord
{
    public int Id { get; set; }
    public string MetricName { get; set; } = null!; // stuff like "avg wait, rating, etc.."
    public double Value { get; set; } // numeric value for the metric
    public DateTime RecordedAt { get; set; } = DateTime.Now;
}