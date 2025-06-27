using Models;

namespace Services;

public abstract class BaseShareChainHandle : IShareChainHandle
{
    protected abstract Task<HandlerResult> ProcessAsync(ResourceShareContext context);

    protected virtual void Validate(ResourceShareContext context) {}

    public async Task<HandlerResult> HandleAsync(ResourceShareContext context)
    {
        Validate(context);
        return await ProcessAsync(context); 
    }
}