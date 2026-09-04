using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TenThousandHours.Data;

namespace TenThousandHours.Pages;

[Authorize]
public class MediaModel(ApplicationDbContext context, UserManager<IdentityUser> userManager) : PageModel
{
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<IdentityUser> _userManager = userManager;

    public IReadOnlyList<MediaGroup> Groups { get; private set; } = [];

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return;
        }

        var items = await _context.MediaItems
            .AsNoTracking()
            .Where(m => m.DailyEntry.UserId == user.Id)
            .OrderByDescending(m => m.DailyEntry.EntryDate)
            .Select(m => new MediaRow
            {
                Type = m.Type,
                EntryDate = m.DailyEntry.EntryDate,
                Url = m.Url,
                YouTubeVideoId = m.YouTubeVideoId,
                StorageProvider = m.StorageProvider
            })
            .ToListAsync();

        Groups = items
            .GroupBy(x => x.Type)
            .OrderBy(g => g.Key)
            .Select(g => new MediaGroup
            {
                Type = g.Key,
                Items = g.ToList()
            })
            .ToList();
    }

    public class MediaGroup
    {
        public string Type { get; set; } = string.Empty;
        public List<MediaRow> Items { get; set; } = [];
    }

    public class MediaRow
    {
        public string Type { get; set; } = string.Empty;
        public DateTime EntryDate { get; set; }
        public string? Url { get; set; }
        public string? YouTubeVideoId { get; set; }
        public string StorageProvider { get; set; } = string.Empty;
    }
}
