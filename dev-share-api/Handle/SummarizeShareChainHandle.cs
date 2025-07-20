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
        var prompt = new StringBuilder()
            .AppendLine("Summarize the following article in a semantic-rich way that:")
            .AppendLine("1. Preserves key technical terms, domain-specific vocabulary, and named entities")
            .AppendLine("2. Maintains semantic relationships between concepts")
            .AppendLine("3. Uses clear, factual language without metaphors or ambiguous terms")
            .AppendLine("4. Includes important numerical values and specific details")
            .AppendLine("5. Maximum length: 100 words")
            .AppendLine("6. Format: Single paragraph, no bullets or sections")
            .AppendLine()
            .AppendLine("Article to summarize:")
            .AppendLine($"{context.ExtractResult}")
            .AppendLine()
            .AppendLine("Return only the summary without any additional text, explanations, or formatting.")
            .ToString();

        var summary = await _summaryService.SummarizeAsync(prompt);
        context.Summary = summary;
        return HandlerResult.Success();
    }
}