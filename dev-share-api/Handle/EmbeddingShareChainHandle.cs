using Models;
using Qdrant.Client.Grpc;

namespace Services;

public class EmbeddingShareChainHandle : BaseShareChainHandle
{

    private readonly IEmbeddingService _embeddingService;

    public EmbeddingShareChainHandle(IEmbeddingService embeddingService)
    {
        _embeddingService = embeddingService;
    }

    protected override void Validate(ResourceShareContext context)
    {
        if (string.IsNullOrWhiteSpace(context.Summary))
        {
            throw new ArgumentNullException(nameof(context.Summary), "Prompt cannot be null or empty.");
        }
    }

    protected async override Task<HandlerResult> ProcessAsync(ResourceShareContext context)
    {
        context.ResourceVectors = await GetVectors(context.Summary);
        
        if (!string.IsNullOrWhiteSpace(context.Insight))
        {
            context.InsightVectors = await GetVectors(context.Insight);
        }

        return HandlerResult.Success();
    }

    private async Task<Dictionary<string, Vector>> GetVectors(string text)
    {
        var denseEmbedding = await _embeddingService.GetDenseEmbeddingAsync(text);
        var (indices, values) = await _embeddingService.GetSparseEmbeddingAsync(text);

        var denseVector = new DenseVector();
        denseVector.Data.AddRange(denseEmbedding);

        var sparseVector = new SparseVector();
        sparseVector.Indices.AddRange(indices);
        sparseVector.Values.AddRange(values);

        var vectors = new Dictionary<string, Vector>
        {
            ["dense_vector"] = new() { Dense = denseVector },
            ["sparse_vector"] = new() { Sparse = sparseVector }
        };

        return vectors;
    }
}
