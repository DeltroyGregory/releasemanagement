using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using rmp.Models;

namespace rmp.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Release> Releases => Set<Release>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<ReleaseSystem> ReleaseSystems => Set<ReleaseSystem>();
    public DbSet<AppVersion> AppVersions => Set<AppVersion>();
    public DbSet<FixVersion> FixVersions => Set<FixVersion>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<LookupItem> Lookups => Set<LookupItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Release>().Property(r => r.ReleaseType).HasConversion<string>();

        builder.Entity<RolePermission>()
            .HasIndex(rp => new { rp.RoleName, rp.PermissionKey })
            .IsUnique();

        builder.Entity<LookupItem>()
            .HasIndex(l => new { l.Category, l.Value })
            .IsUnique();

        builder.Entity<TaskItem>()
            .HasOne(t => t.Release)
            .WithMany(r => r.Tasks)
            .HasForeignKey(t => t.ReleaseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Four independent optional references into the same lookup table. SQL Server refuses more
        // than one cascading (CASCADE/SET NULL) path from the same table into the same target table
        // — "may cause cycles or multiple cascade paths" — even though these four columns are
        // logically independent, so NoAction here; LookupsController.Delete clears any referencing
        // tasks in application code before removing the lookup row instead.
        builder.Entity<TaskItem>()
            .HasOne(t => t.Type)
            .WithMany()
            .HasForeignKey(t => t.TypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<TaskItem>()
            .HasOne(t => t.Component)
            .WithMany()
            .HasForeignKey(t => t.ComponentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<TaskItem>()
            .HasOne(t => t.AppName)
            .WithMany()
            .HasForeignKey(t => t.AppNameId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<TaskItem>()
            .HasOne(t => t.Version)
            .WithMany()
            .HasForeignKey(t => t.VersionId)
            .OnDelete(DeleteBehavior.NoAction);

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
