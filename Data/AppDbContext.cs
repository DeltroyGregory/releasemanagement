using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using mbm.Models;

namespace mbm.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<Release> Releases => Set<Release>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<ReleaseSystem> ReleaseSystems => Set<ReleaseSystem>();
    public DbSet<AppVersion> AppVersions => Set<AppVersion>();
    public DbSet<FixVersion> FixVersions => Set<FixVersion>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Release>().Property(r => r.ReleaseType).HasConversion<string>();

        builder.Entity<TaskItem>()
            .HasOne(t => t.Release)
            .WithMany(r => r.Tasks)
            .HasForeignKey(t => t.ReleaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ReleaseSystem>()
            .HasOne(rs => rs.Release)
            .WithMany(r => r.ReleaseSystems)
            .HasForeignKey(rs => rs.ReleaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<FixVersion>()
            .HasOne(f => f.Release)
            .WithMany(r => r.FixVersions)
            .HasForeignKey(f => f.ReleaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Comment>()
            .HasOne(c => c.Release)
            .WithMany(r => r.Comments)
            .HasForeignKey(c => c.ReleaseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
