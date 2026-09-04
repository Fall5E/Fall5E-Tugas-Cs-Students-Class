using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TenThousandHours.Data;
using TenThousandHours.Models;

namespace TenThousandHours.Pages;

[Authorize]
public class TodayModel(ApplicationDbContext context, UserManager<IdentityUser> userManager) : PageModel
{
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<IdentityUser> _userManager = userManager;

    [BindProperty]
    public EntryInput Input { get; set; } = new();

    public string StatusMessage { get; private set; } = string.Empty;

    public async Task OnGetAsync()
    {
        await LoadTodayAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var today = DateTime.UtcNow.Date;
        var entry = await _context.DailyEntries
            .Include(e => e.Activities)
            .Include(e => e.MediaItems)
            .SingleOrDefaultAsync(e => e.UserId == user.Id && e.EntryDate == today);

        if (entry is null)
        {
            entry = new DailyEntry
            {
                UserId = user.Id,
                EntryDate = today
            };
            _context.DailyEntries.Add(entry);
        }

        entry.WhatHappened = Input.WhatHappened.Trim();
        entry.GratefulFor = Input.GratefulFor.Trim();
        entry.WhatILearned = Input.WhatILearned.Trim();
        entry.WhatMadeTodayMeaningful = Input.WhatMadeTodayMeaningful.Trim();
        entry.WorkNotes = Input.WorkNotes.Trim();
        entry.LearningTopic = Input.LearningTopic.Trim();
        entry.IncomeSource = Input.IncomeSource.Trim();
        entry.IncomeAmount = Input.IncomeAmount;
        entry.ContextWhere = Input.ContextWhere.Trim();
        entry.ContextWith = Input.ContextWith.Trim();

        entry.Activities.Clear();
        foreach (var activity in ParseActivities(Input.ActivitiesText))
        {
            entry.Activities.Add(activity);
        }

        entry.MediaItems.Clear();
        foreach (var photoUrl in ParseLines(Input.PhotoUrls))
        {
            entry.MediaItems.Add(new MediaItem
            {
                Type = "Photo",
                StorageProvider = "cloud-storage",
                Url = photoUrl
            });
        }

        if (!string.IsNullOrWhiteSpace(Input.FaceOfDayUrl))
        {
            entry.MediaItems.Add(new MediaItem
            {
                Type = "FaceOfDay",
                StorageProvider = "cloud-storage",
                Url = Input.FaceOfDayUrl.Trim(),
                Caption = "Face of the Day"
            });
        }

        foreach (var raw in ParseLines(Input.DocumentaryYouTubeIds))
        {
            entry.MediaItems.Add(new MediaItem
            {
                Type = "Documentary",
                StorageProvider = "youtube",
                YouTubeVideoId = NormalizeYouTubeId(raw),
                Url = raw
            });
        }

        var goalTitles = ParseLines(Input.ActiveGoalsText).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var existingGoals = await _context.Goals.Where(g => g.UserId == user.Id).ToListAsync();
        foreach (var goal in existingGoals)
        {
            goal.IsActive = false;
        }

        foreach (var title in goalTitles)
        {
            var existing = existingGoals.FirstOrDefault(g => string.Equals(g.Title, title, StringComparison.OrdinalIgnoreCase));
            if (existing is null)
            {
                _context.Goals.Add(new Goal
                {
                    UserId = user.Id,
                    Title = title,
                    IsActive = true
                });
            }
            else
            {
                existing.IsActive = true;
            }
        }

        await _context.SaveChangesAsync();

        StatusMessage = "Saved today's entry.";
        await LoadTodayAsync();
        return Page();
    }

    private async Task LoadTodayAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return;
        }

        var today = DateTime.UtcNow.Date;
        var entry = await _context.DailyEntries
            .Include(e => e.Activities)
            .Include(e => e.MediaItems)
            .SingleOrDefaultAsync(e => e.UserId == user.Id && e.EntryDate == today);

        var activeGoals = await _context.Goals
            .Where(g => g.UserId == user.Id && g.IsActive)
            .OrderBy(g => g.Title)
            .Select(g => g.Title)
            .ToListAsync();

        if (entry is null)
        {
            Input = new EntryInput
            {
                ActiveGoalsText = string.Join(Environment.NewLine, activeGoals)
            };
            return;
        }

        Input = new EntryInput
        {
            WhatHappened = entry.WhatHappened,
            GratefulFor = entry.GratefulFor,
            WhatILearned = entry.WhatILearned,
            WhatMadeTodayMeaningful = entry.WhatMadeTodayMeaningful,
            WorkNotes = entry.WorkNotes,
            LearningTopic = entry.LearningTopic,
            IncomeSource = entry.IncomeSource,
            IncomeAmount = entry.IncomeAmount,
            ContextWhere = entry.ContextWhere,
            ContextWith = entry.ContextWith,
            ActivitiesText = string.Join(Environment.NewLine, entry.Activities.Select(a => $"{a.Name}|{a.Hours}")),
            PhotoUrls = string.Join(Environment.NewLine, entry.MediaItems.Where(m => m.Type == "Photo").Select(m => m.Url)),
            FaceOfDayUrl = entry.MediaItems.FirstOrDefault(m => m.Type == "FaceOfDay")?.Url ?? string.Empty,
            DocumentaryYouTubeIds = string.Join(Environment.NewLine, entry.MediaItems.Where(m => m.Type == "Documentary").Select(m => m.YouTubeVideoId ?? m.Url)),
            ActiveGoalsText = string.Join(Environment.NewLine, activeGoals)
        };
    }

    private static IEnumerable<ActivityRecord> ParseActivities(string text)
    {
        foreach (var line in ParseLines(text))
        {
            var parts = line.Split('|', 2, StringSplitOptions.TrimEntries);
            if (parts.Length != 2)
            {
                continue;
            }

            if (!decimal.TryParse(parts[1], NumberStyles.Number, CultureInfo.InvariantCulture, out var hours) &&
                !decimal.TryParse(parts[1], out hours))
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(parts[0]) || hours <= 0)
            {
                continue;
            }

            yield return new ActivityRecord
            {
                Name = parts[0],
                Hours = hours
            };
        }
    }

    private static IEnumerable<string> ParseLines(string text) =>
        text.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    private static string NormalizeYouTubeId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var input = value.Trim();
        if (!Uri.TryCreate(input, UriKind.Absolute, out var uri))
        {
            return input;
        }

        if (uri.Host.Contains("youtu.be", StringComparison.OrdinalIgnoreCase))
        {
            return uri.AbsolutePath.Trim('/');
        }

        var query = uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries);
        foreach (var pair in query)
        {
            var keyValue = pair.Split('=', 2);
            if (keyValue.Length == 2 && keyValue[0] == "v")
            {
                return keyValue[1];
            }
        }

        return input;
    }

    public class EntryInput
    {
        [Display(Name = "What happened today?")]
        public string WhatHappened { get; set; } = string.Empty;

        [Display(Name = "What are you grateful for?")]
        public string GratefulFor { get; set; } = string.Empty;

        [Display(Name = "What did you learn?")]
        public string WhatILearned { get; set; } = string.Empty;

        [Display(Name = "What made today meaningful?")]
        public string WhatMadeTodayMeaningful { get; set; } = string.Empty;

        [Display(Name = "Activities (Name|Hours per line)")]
        public string ActivitiesText { get; set; } = string.Empty;

        [Display(Name = "Work notes")]
        public string WorkNotes { get; set; } = string.Empty;

        [Display(Name = "Learning topic")]
        public string LearningTopic { get; set; } = string.Empty;

        [Display(Name = "Income source")]
        public string IncomeSource { get; set; } = string.Empty;

        [Display(Name = "Income amount")]
        public decimal? IncomeAmount { get; set; }

        [Display(Name = "Where")]
        public string ContextWhere { get; set; } = string.Empty;

        [Display(Name = "With")]
        public string ContextWith { get; set; } = string.Empty;

        [Display(Name = "Current goals (one per line)")]
        public string ActiveGoalsText { get; set; } = string.Empty;

        [Display(Name = "Photo URLs (one per line)")]
        public string PhotoUrls { get; set; } = string.Empty;

        [Display(Name = "Face of Day URL")]
        public string FaceOfDayUrl { get; set; } = string.Empty;

        [Display(Name = "Documentary YouTube IDs or URLs (one per line)")]
        public string DocumentaryYouTubeIds { get; set; } = string.Empty;
    }
}
