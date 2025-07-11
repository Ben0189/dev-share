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
        if (context.ResourceVectors == null || context.ResourceVectors.Count == 0)
        {
            throw new ArgumentNullException(nameof(context.ResourceVectors), "Vectors cannot be null or empty.");
        }
    }

    protected async override Task<HandlerResult> ProcessAsync(ResourceShareContext context)
    {
        var resourceId = Guid.NewGuid().ToString();
        if (!string.IsNullOrWhiteSpace(context.Summary))
        {
            await _vectorService.UpsertResourceAsync(resourceId, context.Url, context.Summary, context.ResourceVectors);
        }
        if (!string.IsNullOrWhiteSpace(context.Insight))
        {
            var insightId = Guid.NewGuid().ToString();
            await _vectorService.UpsertInsightAsync(insightId, context.Url, context.Insight, resourceId, context.InsightVectors);
        }
        return HandlerResult.Success();
    }

}