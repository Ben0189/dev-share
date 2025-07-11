using Qdrant.Client;
using Qdrant.Client.Grpc;
using Services;
using Executor;
using Azure.AI.OpenAI;
using System.ClientModel;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace Configuration;

public static class DIServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Config sections
        var openAiConfig = configuration.GetSection("OpenAI");
        var qdrantConfig = configuration.GetSection("Qdrant");

        // Qdrant Client
        services.AddSingleton<QdrantClient>(_ =>
        {
            var channel = QdrantChannel.ForAddress(
                $"{qdrantConfig["Host"]}:6334",
                new ClientConfiguration { ApiKey = qdrantConfig["ApiKey"] }
            );
            return new QdrantClient(new QdrantGrpcClient(channel));
        });

        // OpenAI Client
        services.AddSingleton<AzureOpenAIClient>(_ =>
        {
            var apiKey = openAiConfig["ApiKey"]
                         ?? throw new InvalidOperationException("OpenAI:ApiKey is missing");
            var endpoint = openAiConfig["Endpoint"]
                         ?? throw new InvalidOperationException("OpenAI:Endpoint is missing");
            return new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apiKey));
        });

        // Database
        services.AddDbContext<DevShareDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // HTTP Client
        services.AddHttpClient("FastEmbed", client =>
        {
            client.BaseAddress = new Uri("https://python-ai-model-acgfbgfbffb0fyav.australiaeast-01.azurewebsites.net");
        });

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Application Services
        services.AddScoped<IEmbeddingService, EmbeddingService>();
        services.AddScoped<IVectorService, VectorService>();
        services.AddScoped<ISummaryService, SummaryService>();
        services.AddScoped<IOnlineResearchService, OnlineResearchService>();
        services.AddScoped<IResourceService, ResourceService>();

        // Not allowed to alter the sort of the following code. 
        services.AddScoped<ShareChainExecutor>();
        services.AddScoped<IShareChainHandle, SummarizeShareChainHandle>();
        services.AddScoped<IShareChainHandle, EmbeddingShareChainHandle>();
        services.AddScoped<IShareChainHandle, VectorShareChainHandle>();

        return services;
    }

    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "DevShare API", Version = "v1" });
        });

        return services;
    }

    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins(
                    "http://localhost:3000",
                    "https://dev-share-ui-hce9cxaxacc8fahu.australiaeast-01.azurewebsites.net"
                )
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
        });

        return services;
    }
}