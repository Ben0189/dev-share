using Models;

namespace Services;

public class SummarizeShareChainHandle : BaseShareChainHandle
{
    private readonly ISummaryService _summaryService;

    public SummarizeShareChainHandle(ISummaryService summaryService)
    {
        _summaryService = summaryService;
    }

    protected override void Validate(ResourceShareContext context)
    {
        // if (context.ExistingResource == null && string.IsNullOrWhiteSpace(context.Prompt))
        //     throw new ArgumentNullException(nameof(context.Prompt), "Prompt cannot be null or empty.");
    }

    public override async Task<bool> IsSkip(ResourceShareContext context)
    {
        return context.ExistingResource != null;
    }


    protected override async Task<HandlerResult> ProcessAsync(ResourceShareContext context)
    {
        var summary = await _summaryService.SummarizeAsync(context.Prompt);
        context.Summary = summary;
        return HandlerResult.Success();
    }
}