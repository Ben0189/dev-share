using Newtonsoft.Json;

namespace Models;
public class EmbeddingResponse
{

    [JsonProperty("embeddings")]
    public List<float[]>? Embeddings { get; set; }
}

    public class SparseEmbeddingResponse
    {
        public List<SparseEmbedding>? Embeddings { get; set; }
    }

    public class SparseEmbedding
    {
        public uint[]? Indices { get; set; }
        public float[]? Values { get; set; }
    }