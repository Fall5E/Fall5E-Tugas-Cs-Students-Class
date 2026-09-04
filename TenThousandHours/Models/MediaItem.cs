namespace TenThousandHours.Models;

public class MediaItem
{
    public int Id { get; set; }
    public int DailyEntryId { get; set; }
    public DailyEntry DailyEntry { get; set; } = null!;

    public string Type { get; set; } = string.Empty; // Photo | FaceOfDay | Documentary
    public string StorageProvider { get; set; } = string.Empty;
    public string? StorageKey { get; set; }
    public string? Url { get; set; }
    public string? YouTubeVideoId { get; set; }
    public string? Caption { get; set; }
}
