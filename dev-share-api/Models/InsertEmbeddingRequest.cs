using Qdrant.Client.Grpc;

namespace Models;

public class InsertEmbeddingRequest
{
    public required string Url { get; set; }
    public required string Text { get; set; }
    public required string NoteId { get; set; }
    public required Dictionary<string, Vector> Vectors { get; set; }
}