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
        // if (string.IsNullOrWhiteSpace(context.Summary))
        //     throw new ArgumentNullException(nameof(context.Summary), "Prompt cannot be null or empty.");
    }

    protected override async Task<HandlerResult> ProcessAsync(ResourceShareContext context)
    {
        var denseEmbedding = await _embeddingService.GetDenseEmbeddingAsync(context.Summary);
        var (indices, values) = await _embeddingService.GetSparseEmbeddingAsync(context.Summary);

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

        context.ResourceVectors = vectors;
        return HandlerResult.Success();
    }
}