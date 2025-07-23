using Models;

namespace Services;

public abstract class BaseShareChainHandle : IShareChainHandle
{
    public async Task<HandlerResult> HandleAsync(ResourceShareContext context)
    {
        Validate(context);
        return await ProcessAsync(context);
    }

    protected abstract Task<HandlerResult> ProcessAsync(ResourceShareContext context);

    protected virtual void Validate(ResourceShareContext context)
    {
    }

    public virtual async Task<bool> IsSkip(ResourceShareContext context)
    {
        return false; 
    }
}