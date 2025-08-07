using Azure.AI.OpenAI;
using System.Text.Json;
using Models;
using OpenAI.Chat;

namespace Services;

public interface IOnlineResearchService
{
    Task<IEnumerable<ResourceDto>> PerformOnlineResearchAsync(string query, int topK);
}

public class OnlineResearchService : IOnlineResearchService
{
    private readonly AzureOpenAIClient _client;
    private const string _deploymentName = "gpt-4o-mini";
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public OnlineResearchService(AzureOpenAIClient openAIClient)
    {
        _client = openAIClient ?? throw new ArgumentNullException(nameof(openAIClient));
    }

    public async Task<IEnumerable<ResourceDto>> PerformOnlineResearchAsync(string query, int topK = 3)
    {
        if (string.IsNullOrWhiteSpace(query))
            throw new ArgumentException("Query cannot be empty", nameof(query));

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage($@"
                You are an AI research assistant. Your task is to return an array of up to {topK} concise and factual resources relevant to the user's query.

                For each resource, provide:
                - Title: A title for the answer of the summary in less than 15 words.
                - Content: A concise factual answer or summary.
                - Url: A direct, relevant web source.

                Always call the `generate_research_results` function with your result in JSON:
                {{
                    ""results"": [
                        {{
                            ""title"": string, 
                            ""content"": string, 
                            ""url"": string
                        }}
                    ]
                }}

                Guidelines:
                - No explanations or formatting.
                - Never return plain text; always structured JSON using the function.
                - Results must be unique and from reputable sources.
            "),
            new UserChatMessage(query)
        };

        var tool = CreateGenerateResearchResultsTool(topK);

        return await CallToolAndDeserializeAsync<ResourceResultWrapper>(
            toolFunctionName: "generate_research_results",
            messages: messages,
            tool: tool
        ).ContinueWith(t => t.Result?.Results ?? new List<ResourceDto>()); ;
    }

    private ChatTool CreateGenerateResearchResultsTool(int topK)
    {
        return ChatTool.CreateFunctionTool(
            functionName: "generate_research_results",
            functionDescription: $"Returns up to {topK} concise and factual research results for the given query.",
            functionParameters: BinaryData.FromObjectAsJson(new
            {
                type = "object",
                properties = new
                {
                    results = new
                    {
                        type = "array",
                        items = new
                        {
                            type = "object",
                            properties = new
                            {
                                title = new { type = "string", description = "Title" },
                                content = new { type = "string", description = "Concise, factual answer or summary." },
                                url = new { type = "string", description = "Direct relevant web source." }
                            },
                            required = new[] { "content", "url" }
                        },
                        minItems = 1,
                        maxItems = topK
                    }
                },
                required = new[] { "results" }
            })
        );
    }

    public async Task<T> CallToolAndDeserializeAsync<T>(
        string toolFunctionName,
        List<ChatMessage> messages,
        ChatTool tool)
    {
        var client = _client.GetChatClient(deploymentName: _deploymentName);
        ChatCompletionOptions options = new()
        {
            Tools = { tool }
        };
        ChatCompletion response = await client.CompleteChatAsync(messages, options);

        var toolCall = response.ToolCalls.FirstOrDefault(tc => tc.FunctionName == toolFunctionName);
        if (toolCall == null)
            throw new InvalidOperationException("No function call response found.");

        var jsonRes = toolCall.FunctionArguments.ToString();
        var cleanedResponse = jsonRes
                .Replace("```json", "")
                .Replace("```", "")
                .Replace("\\n", "")
                .Replace("\n", "")
                .Trim();
        var result = JsonSerializer.Deserialize<T>(cleanedResponse, _jsonOptions);
        if (result == null)
            throw new InvalidOperationException("Deserialization failed.");

        return result;
    }

    private class ResourceResultWrapper
    {
        public List<ResourceDto> Results { get; set; } = new();
    }
}
