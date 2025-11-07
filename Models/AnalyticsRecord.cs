namespace MediScope.Models;

public class AnalyticsRecord
{
    public int Id { get; set; }
    public string MetricName { get; set; } = null!; // stuff like "avg wait, rating, etc.."
    public double Value { get; set; } // numeric value for the metric
    public DateTime RecordedAt { get; set; } = DateTime.Now;
}