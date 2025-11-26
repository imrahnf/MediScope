namespace MediScope.Models.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalDoctors { get; set; }
    public int TotalPatients { get; set; }
    public double AverageRating { get; set; }
    public IEnumerable<AnalyticsRecord> RecentRecords { get; set; } = new List<AnalyticsRecord>();
}
