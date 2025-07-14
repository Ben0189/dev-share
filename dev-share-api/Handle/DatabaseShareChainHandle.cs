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
        var resourceId = IdGeneratorUtil.GetNextId().ToString();

        if (context.ExistingResource == null)
        {
            await _vectorService.UpsertResourceAsync(
                context.Url!,
                resourceId,
                context.Summary!,
                context.ResourceVectors!);
        }

        await _vectorService.UpsertInsightAsync(
            IdGeneratorUtil.GetNextId().ToString(),
            context.Url!,
            context.Insight!,
            resourceId,
            context.InsightVectors!);
        return HandlerResult.Success();
    }
}