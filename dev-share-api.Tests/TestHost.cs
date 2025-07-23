using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class TestHost
{
    public static IServiceProvider BuildTestServiceProvider()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.local.json", optional: true)
            .Build();

        var services = new ServiceCollection();
        services.AddInfrastructureServices(config)
                .AddApplicationServices();

        return services.BuildServiceProvider();
    }
}