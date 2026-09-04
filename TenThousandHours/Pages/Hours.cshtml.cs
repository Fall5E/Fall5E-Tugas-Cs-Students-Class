using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TenThousandHours.Data;

namespace TenThousandHours.Pages;

[Authorize]
public class HoursModel(ApplicationDbContext context, UserManager<IdentityUser> userManager) : PageModel
{
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<IdentityUser> _userManager = userManager;

    public decimal TotalHours { get; private set; }
    public decimal ProgressPercent => TotalHours <= 0 ? 0 : Math.Min(100, TotalHours / 10000m * 100m);
    public IReadOnlyList<ActivityTotal> ActivityTotals { get; private set; } = [];

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return;
        }

        ActivityTotals = await _context.ActivityRecords
            .AsNoTracking()
            .Where(a => a.DailyEntry.UserId == user.Id)
            .GroupBy(a => a.Name)
            .Select(g => new ActivityTotal
            {
                ActivityName = g.Key,
                Hours = g.Sum(x => x.Hours)
            })
            .OrderByDescending(x => x.Hours)
            .ToListAsync();

        TotalHours = ActivityTotals.Sum(x => x.Hours);
    }

    public class ActivityTotal
    {
        public string ActivityName { get; set; } = string.Empty;
        public decimal Hours { get; set; }
    }
}
