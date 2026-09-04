using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TenThousandHours.Models;

namespace TenThousandHours.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Goal> Goals => Set<Goal>();
    public DbSet<DailyEntry> DailyEntries => Set<DailyEntry>();
    public DbSet<ActivityRecord> ActivityRecords => Set<ActivityRecord>();
    public DbSet<MediaItem> MediaItems => Set<MediaItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Goal>()
            .HasIndex(g => new { g.UserId, g.Title });

        builder.Entity<DailyEntry>()
            .HasIndex(e => new { e.UserId, e.EntryDate })
            .IsUnique();

        builder.Entity<DailyEntry>()
            .Property(e => e.EntryDate)
            .HasConversion(v => v.Date, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        builder.Entity<DailyEntry>()
            .HasMany(e => e.Activities)
            .WithOne(a => a.DailyEntry)
            .HasForeignKey(a => a.DailyEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<DailyEntry>()
            .HasMany(e => e.MediaItems)
            .WithOne(m => m.DailyEntry)
            .HasForeignKey(m => m.DailyEntryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
