namespace Models;

public class SearchEmbeddingRequest
{
    public required float[] QueryEmbedding { get; set; }
    public int TopRelatives { get; set; }
    public required string queryText { get; set; }
}