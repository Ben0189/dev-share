using Models;

namespace Services;

public class VectorShareChainHandle : BaseShareChainHandle
{

    private readonly IVectorService _vectorService;

    public VectorShareChainHandle(IVectorService vectorService)
    {
        _vectorService = vectorService;
    }

    protected override void Validate(ResourceShareContext context)
    {
        if (context.Vectors == null || context.Vectors.Count == 0)
        {
            throw new ArgumentNullException(nameof(context.Vectors), "Vectors cannot be null or empty.");
        }
    }

    protected async override Task<HandlerResult> ProcessAsync(ResourceShareContext context)
    {
        await _vectorService.UpsertEmbeddingAsync(context.Url, IdGenerator.GetNextId().ToString(), context.Summary, context.Vectors);
        return HandlerResult.Success();
    }
    
}