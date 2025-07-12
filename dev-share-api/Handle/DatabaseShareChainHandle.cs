using Models;

namespace Services;

public class DatabaseShareChainHandle : BaseShareChainHandle
{
    private readonly IVectorService _vectorService;

    public DatabaseShareChainHandle(IVectorService vectorService)
    {
        _vectorService = vectorService;
    }

    protected override void Validate(ResourceShareContext context)
    {
        // if (context.ResourceVectors == null || context.ResourceVectors.Count == 0)
        //     throw new ArgumentNullException(nameof(context.ResourceVectors), "Vectors cannot be null or empty.");
    }

    protected override async Task<HandlerResult> ProcessAsync(ResourceShareContext context)
    {
        await _vectorService.UpsertEmbeddingAsync(context.Url, IdGeneratorUtil.GetNextId().ToString(), context.Summary,
            context.ResourceVectors);
        return HandlerResult.Success();
    }
}