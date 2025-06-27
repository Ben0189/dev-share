namespace Services;

public interface ISummaryService
{
    Task<string> SummarizeAsync(string article);
}