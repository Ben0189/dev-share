using Models;

namespace Services;

public class DatabaseShareChainHandle : BaseShareChainHandle
{
    private readonly IVectorService _vectorService;
    private readonly IUserInsightService _userInsightService;
    private readonly IResourceService _resourceService;

    public DatabaseShareChainHandle(IVectorService vectorService, IUserInsightService userInsightService, IResourceService resourceService)
    {
        _vectorService = vectorService;
        _userInsightService = userInsightService;
        _resourceService = resourceService;
    }

    protected override void Validate(ResourceShareContext context)
    {
        // if (context.ResourceVectors == null || context.ResourceVectors.Count == 0)
        //     throw new ArgumentNullException(nameof(context.ResourceVectors), "Vectors cannot be null or empty.");
    }

    protected override async Task<HandlerResult> ProcessAsync(ResourceShareContext context)
    {

        var resourceId = 0L;

        if (context.ExistingResource == null)
        {
            resourceId = IdGeneratorUtil.GetNextId();
            await _resourceService.AddResourceAsync(
                new ResourceDto
                {
                    ResourceId = resourceId,
                    Content = context.Summary,
                    Url = context.Url
                });
            await _vectorService.UpsertResourceAsync(
                resourceId.ToString(),
                context.Url!,
                context.Summary!,
                context.ResourceVectors!);
        }
        else
        { 
            resourceId = context.ExistingResource.ResourceId;
        }
        
        await _vectorService.UpsertInsightAsync(
            IdGeneratorUtil.GetNextId().ToString(),
            context.Url!,
            context.Insight!,
            resourceId.ToString(),
            context.InsightVectors!);

        await _userInsightService.AddUserInsightAsync(
            new UserInsightDto
            {
                ResourceId = resourceId,
                Content = context.Insight
            });

        return HandlerResult.Success();
      
    }
}