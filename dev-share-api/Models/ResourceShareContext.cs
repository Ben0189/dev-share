using Qdrant.Client.Grpc;

namespace Models;

public class ResourceShareContext
{
    public string? Url { get; set; }
    public string? Insight { get; set; }
    public string? Summary { get; set; }
    public string? Title { get; set; }
    public Dictionary<string, Vector>? ResourceVectors { get; set; }
    public Dictionary<string, Vector>? InsightVectors { get; set; }
    public ResourceDto? ExistingResource { get; set; }
    public string? ExtractResult { get; set; }
    
}