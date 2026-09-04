using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TenThousandHours.Data;

namespace TenThousandHours.Pages;

[Authorize]
public class ArchiveModel(ApplicationDbContext context, UserManager<IdentityUser> userManager) : PageModel
{
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<IdentityUser> _userManager = userManager;

    [BindProperty(SupportsGet = true)]
    public string Query { get; set; } = string.Empty;

    public IReadOnlyList<ArchiveResult> Results { get; private set; } = [];

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null || string.IsNullOrWhiteSpace(Query))
        {
            return;
        }

        var q = Query.Trim();

        Results = await _context.DailyEntries
            .AsNoTracking()
            .Where(e => e.UserId == user.Id)
            .Where(e =>
                e.WhatHappened.Contains(q) ||
                e.GratefulFor.Contains(q) ||
                e.WhatILearned.Contains(q) ||
                e.WhatMadeTodayMeaningful.Contains(q) ||
                e.WorkNotes.Contains(q) ||
                e.LearningTopic.Contains(q) ||
                e.IncomeSource.Contains(q) ||
                e.ContextWhere.Contains(q) ||
                e.ContextWith.Contains(q) ||
                e.Activities.Any(a => a.Name.Contains(q)) ||
                e.MediaItems.Any(m =>
                    (m.Caption ?? string.Empty).Contains(q) ||
                    (m.Url ?? string.Empty).Contains(q) ||
                    (m.YouTubeVideoId ?? string.Empty).Contains(q)))
            .OrderByDescending(e => e.EntryDate)
            .Select(e => new ArchiveResult
            {
                EntryDate = e.EntryDate,
                WhatHappened = e.WhatHappened,
                WorkNotes = e.WorkNotes,
                LearningTopic = e.LearningTopic
            })
            .Take(150)
            .ToListAsync();
    }

    public class ArchiveResult
    {
        public DateTime EntryDate { get; set; }
        public string WhatHappened { get; set; } = string.Empty;
        public string WorkNotes { get; set; } = string.Empty;
        public string LearningTopic { get; set; } = string.Empty;
    }
}
