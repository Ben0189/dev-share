using Models;
using Services;

namespace Executor;

public class ShareChainExecutor
{
    private readonly IEnumerable<IShareChainHandle> _handlers;

    private readonly IResourceService _resourceService;

    public ShareChainExecutor(IEnumerable<IShareChainHandle> handlers, IResourceService resourceService)
    {
        _handlers = handlers;
        _resourceService = resourceService;
    }

    public async Task ExecuteAsync(ResourceShareContext context)
    {
        await preHandle(context);
        foreach (var handler in _handlers)
        {
            // Check if the handler should be skipped
            if (await handler.IsSkip(context))
                continue;
            
            var result = await handler.HandleAsync(context);
            if (!result.IsSuccess)
                return;
        }
    }

    private async Task preHandle(ResourceShareContext context)
    {
        ResourceDto resourceDto = await _resourceService.GetResourceByUrl(UrlManageUtil.NormalizeUrl(context.Url));
        if (resourceDto != null)
        {
            context.ExistingResource = new ResourceDto()
            {
                ResourceId = resourceDto.ResourceId,
                Url = resourceDto.Url,
                NormalizeUrl = resourceDto.NormalizeUrl,
                Content = resourceDto.Content
            };
        }
    }
}