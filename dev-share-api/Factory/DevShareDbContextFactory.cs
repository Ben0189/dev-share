// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Design;
// using Data;

// public class DevShareDbContextFactory : IDesignTimeDbContextFactory<DevShareDbContext>
// {
//     public DevShareDbContext CreateDbContext(string[] args)
//     {
//         var basePath = Directory.GetCurrentDirectory();
        
//         var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        
//         var configuration = new ConfigurationBuilder()
//             .SetBasePath(basePath)
//             .AddJsonFile("appsettings.json", optional: false)
//             .AddJsonFile($"appsettings.{environment}.json", optional: true)
//             .AddJsonFile("appsettings.local.json", optional: true)
//             .AddEnvironmentVariables()
//             .Build();
        
//         var connectionString = configuration.GetConnectionString("DefaultConnection")
//                                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        
//         var optionsBuilder = new DbContextOptionsBuilder<DevShareDbContext>();
//         optionsBuilder.UseSqlServer(connectionString);
//         return new DevShareDbContext(optionsBuilder.Options);
//     }
// }