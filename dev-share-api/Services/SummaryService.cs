using OpenAI;
using OpenAI.Chat;
using OpenAI.Embeddings;

namespace Services;

public class SummaryService : ISummaryService
{
    private readonly OpenAIClient _client;
    private const string chatModelId = "gpt-4o-mini";

    public SummaryService(OpenAIClient openAIClient)
    {
        _client = openAIClient;
    }

    public async Task<string> SummarizeAsync(string prompt)
    {
        ChatCompletion response = await _client.GetChatClient(model: chatModelId).CompleteChatAsync(prompt);
        return response.Content[0].Text;
    }
}
