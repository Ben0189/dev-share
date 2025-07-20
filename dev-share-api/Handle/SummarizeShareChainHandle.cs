using System.Text;
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
        if (string.IsNullOrWhiteSpace(context.ExtractResult))
        {
            return HandlerResult.Fail("No content provided for summarization");
        }

        var summary = await _summaryService.SummarizeAsync(context.ExtractResult);
        context.Summary = summary.Summary;
        context.Title = summary.Title;
        return HandlerResult.Success();
    }

}