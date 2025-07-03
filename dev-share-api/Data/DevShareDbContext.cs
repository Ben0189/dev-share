using Entities;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class DevShareDbContext : DbContext
{
    public DevShareDbContext(DbContextOptions<DevShareDbContext> options)
        : base(options) {}
    
    public DbSet<Resource> Resources { get; set; }
    public DbSet<UserInsight> UserInsights { get; set; }
    
}

