using Models;
using Services;

namespace Executor;

public class ShareChainExecutor
{
    private readonly IEnumerable<IShareChainHandle> _handlers;

    public ShareChainExecutor(IEnumerable<IShareChainHandle> handlers)
    {
        _handlers = handlers;
    }

    public async Task ExecuteAsync(ResourceShareContext context)
    {
        foreach (var handler in _handlers)
        {
            var result = await handler.HandleAsync(context);
            if (!result.IsSuccess)
                return;
        }
    }
}