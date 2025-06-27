using Models;

namespace Services;


public interface IShareChainHandle
{
    Task<HandlerResult> HandleAsync(ResourceShareContext context);
}