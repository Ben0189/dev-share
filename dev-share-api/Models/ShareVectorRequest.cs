using Qdrant.Client.Grpc;

namespace Models;

public class ShareVectorRequest
{
    public string ResourceId { get; set; }
    public string Url { get; set; }
    public string Content { get; set; }
    public Dictionary<string, Vector> Vectors { get; set; }
}