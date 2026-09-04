namespace TenThousandHours.Models;

public class DailyEntry
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime EntryDate { get; set; }

    public string WhatHappened { get; set; } = string.Empty;
    public string GratefulFor { get; set; } = string.Empty;
    public string WhatILearned { get; set; } = string.Empty;
    public string WhatMadeTodayMeaningful { get; set; } = string.Empty;

    public string WorkNotes { get; set; } = string.Empty;
    public string LearningTopic { get; set; } = string.Empty;
    public string IncomeSource { get; set; } = string.Empty;
    public decimal? IncomeAmount { get; set; }
    public string ContextWhere { get; set; } = string.Empty;
    public string ContextWith { get; set; } = string.Empty;

    public ICollection<ActivityRecord> Activities { get; set; } = new List<ActivityRecord>();
    public ICollection<MediaItem> MediaItems { get; set; } = new List<MediaItem>();
}
