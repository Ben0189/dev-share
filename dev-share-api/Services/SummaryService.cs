using Azure.AI.OpenAI;
using OpenAI.Chat;

namespace Services;

public interface ISummaryService
{
    Task<string> SummarizeAsync(string article);
}

public class SummaryService : ISummaryService
{
    private readonly AzureOpenAIClient _client;
    private const string _deploymentName = "gpt-4o-mini";

    public SummaryService(AzureOpenAIClient openAIClient)
    {
        _client = openAIClient;
    }

    public async Task<string> SummarizeAsync(string prompt)
    {
        ChatCompletion response = await _client.GetChatClient(deploymentName: _deploymentName).CompleteChatAsync(prompt);
        return response.Content[0].Text;
    }
}
