using Entities;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class DevShareDbContext : DbContext
{
    public DevShareDbContext(DbContextOptions<DevShareDbContext> options)
        : base(options)
    {
    }

    public DbSet<Resource> Resources { get; set; }
    public DbSet<UserInsight> UserInsights { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserInsight>()
            .HasOne(ui => ui.Resource)
            .WithMany(r => r.UserInsights)
            .HasForeignKey(ui => ui.ResourceId)
            .HasPrincipalKey(r => r.ResourceId); 

        base.OnModelCreating(modelBuilder);
    }
}