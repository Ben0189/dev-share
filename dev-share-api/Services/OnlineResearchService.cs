using Azure.AI.OpenAI;
using OpenAI.Chat;

namespace Services;
public interface IOnlineResearchService
{
    Task<string> PerformOnlineResearchAsync(string query);
}

public class OnlineResearchService : IOnlineResearchService
{
    private readonly AzureOpenAIClient _client;
    private readonly string _deploymentName = "gpt-4o-mini"; // Set this to your deployment name

    public OnlineResearchService(AzureOpenAIClient openAIClient)
    {
        _client = openAIClient;
    }

    public async Task<string> PerformOnlineResearchAsync(string query)
    {
        // Fallback: use basic completions if chat completions types are not available
        var Prompts = $"You are an AI assistant that performs online research and returns concise, factual answers. User query: {query}";


        ChatCompletion response = await _client.GetChatClient(_deploymentName).CompleteChatAsync(Prompts);
        return response.Content[0].Text;
    }
}
