using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TenThousandHours.Data;

namespace TenThousandHours.Pages;

[Authorize]
public class TimelineModel(ApplicationDbContext context, UserManager<IdentityUser> userManager) : PageModel
{
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<IdentityUser> _userManager = userManager;

    public IReadOnlyList<TimelineEntry> Entries { get; private set; } = [];

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return;
        }

        Entries = await _context.DailyEntries
            .AsNoTracking()
            .Where(e => e.UserId == user.Id)
            .Include(e => e.Activities)
            .Include(e => e.MediaItems)
            .OrderByDescending(e => e.EntryDate)
            .Select(e => new TimelineEntry
            {
                EntryDate = e.EntryDate,
                WhatHappened = e.WhatHappened,
                WorkNotes = e.WorkNotes,
                LearningTopic = e.LearningTopic,
                IncomeAmount = e.IncomeAmount,
                ActivitySummary = e.Activities.Select(a => a.Name + " " + a.Hours + "h").ToList(),
                HasFaceOfDay = e.MediaItems.Any(m => m.Type == "FaceOfDay")
            })
            .Take(400)
            .ToListAsync();
    }

    public class TimelineEntry
    {
        public DateTime EntryDate { get; set; }
        public string WhatHappened { get; set; } = string.Empty;
        public string WorkNotes { get; set; } = string.Empty;
        public string LearningTopic { get; set; } = string.Empty;
        public decimal? IncomeAmount { get; set; }
        public List<string> ActivitySummary { get; set; } = [];
        public bool HasFaceOfDay { get; set; }
    }
}
