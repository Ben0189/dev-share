namespace DevShare.Api.Configuration;

public class VectorDbSettings
{
    public const string SectionName = "VectorDb";
    
    // Collections
    public string ResourceCollection { get; set; } = "BlotzShare_Resource";
    public string InsightCollection { get; set; } = "BlotzShare_Insight";
    
    // Vector dimensions
    public uint Dimensions { get; set; } = 384;  // MiniLM-L6-v2 dimension
    
    // Thresholds
    public float DenseScoreThreshold { get; set; } = 0.65f;    // Balanced semantic similarity
    public float SparseScoreThreshold { get; set; } = 0.15f;   // Good token overlap
    public float HybridScoreThreshold { get; set; } = 0.53f;   // Optimal fusion threshold
    
    // Vector configurations
    public VectorConfig Dense { get; set; } = new();
    public VectorConfig Sparse { get; set; } = new();
}

public class VectorConfig
{
    public string Name { get; set; } = string.Empty;
    public bool OnDisk { get; set; }
    public ulong MinTokenLength { get; set; } = 2;
    public ulong MaxTokenLength { get; set; } = 10;
}