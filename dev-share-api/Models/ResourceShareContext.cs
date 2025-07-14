using Qdrant.Client.Grpc;

namespace Models;

public class ResourceShareContext
{
    public string? Url { get; set; }
    public string? Comment { get; set; }
    public string? Summary { get; set; }
    public Dictionary<string, Vector>? ResourceVectors { get; set; }
    public Dictionary<string, Vector>? UserInsightVectors { get; set; }
    public string? Prompt { get; set; }
    public ResourceDTO? ExistingResource { get; set; }


}