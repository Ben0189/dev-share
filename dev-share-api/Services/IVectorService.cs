using Models;
using Qdrant.Client.Grpc;

namespace Services;

public interface IVectorService
{
    Task InitializeAsync();
    Task<UpdateResult> UpsertEmbeddingAsync(string url, string noteId, string content, Dictionary<string, Vector> vector);
    Task<List<VectorSearchResultDto>> SearchEmbeddingAsync(float[] denseQueryVector, uint[] sparseIndices, float[] sparseValues, int topK);
    Task<UpdateResult> IndexingAsync(string fieldName);
}