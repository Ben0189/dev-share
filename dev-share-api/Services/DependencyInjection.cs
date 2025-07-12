using System.ClientModel;
using Azure.AI.OpenAI;
using Data;
using Executor;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        // Config sections
        var openAiConfig = config.GetSection("OpenAI");
        var qdrantConfig = config.GetSection("Qdrant");

        // Qdrant client
        services.AddSingleton<QdrantClient>(_ =>
        {
            var channel = QdrantChannel.ForAddress(
                $"{qdrantConfig["Host"]}:6334",
                new ClientConfiguration { ApiKey = qdrantConfig["ApiKey"] }
            );
            return new QdrantClient(new QdrantGrpcClient(channel));
        });

        // OpenAI client
        services.AddSingleton<AzureOpenAIClient>(_ =>
        {
            var apiKey = openAiConfig["ApiKey"]
                         ?? throw new InvalidOperationException("OpenAI:ApiKey is missing");
            var endpoint = openAiConfig["Endpoint"]
                           ?? throw new InvalidOperationException("OpenAI:Endpoint is missing");
            return new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apiKey));
        });


        services.AddHttpClient("FastEmbed",
        client =>
        {
            client.BaseAddress = new Uri("https://python-ai-model-acgfbgfbffb0fyav.australiaeast-01.azurewebsites.net");
        });

        // Application services
        services.AddScoped<IVectorService>(sp =>
        {
            var qdrantClient = sp.GetRequiredService<QdrantClient>();
            return new VectorService(qdrantClient);
        });

        services.AddScoped<IEmbeddingService>(sp =>
        {
            var openAiClient = sp.GetRequiredService<AzureOpenAIClient>();
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            return new EmbeddingService(openAiClient, httpClientFactory);
        });

        services.AddScoped<ISummaryService>(sp =>
            {
                var openAiClient = sp.GetRequiredService<AzureOpenAIClient>();
                return new SummaryService(openAiClient);
            });
        
        
        // Sql Server
        services.AddDbContext<DevShareDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        //Not allowed to alter the sort of the following code. 
        services.AddScoped<ShareChainExecutor>();
        services.AddScoped<IShareChainHandle, SummarizeShareChainHandle>();
        services.AddScoped<IShareChainHandle, EmbeddingShareChainHandle>();
        services.AddScoped<IShareChainHandle, DatabaseShareChainHandle>();

        services.AddScoped<IResourceService, ResourceService>();
        services.AddScoped<IUserInsightService, UserInsightService>();

        return services;
    }
}