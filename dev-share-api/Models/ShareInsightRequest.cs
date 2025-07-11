
using Qdrant.Client.Grpc;

namespace Models;
public class ShareInsightRequest
{
    public string InsightId { get; set; }
    public string Url { get; set; }
    public string Content { get; set; }
    public string ResourceId { get; set; }
    public Dictionary<string, Vector> Vectors { get; set; }
}