namespace TenThousandHours.Models;

public class ActivityRecord
{
    public int Id { get; set; }
    public int DailyEntryId { get; set; }
    public DailyEntry DailyEntry { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public decimal Hours { get; set; }
}
