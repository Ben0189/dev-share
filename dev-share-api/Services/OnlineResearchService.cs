using Azure.AI.OpenAI;
using System.Text.Json;
using Models;
using OpenAI.Chat;

namespace Services;

public interface IOnlineResearchService
{
    Task<IEnumerable<VectorResourceDto>> PerformOnlineResearchAsync(string query, int topK);
}

public class OnlineResearchService : IOnlineResearchService
{
    private readonly AzureOpenAIClient _client;
    private readonly string _deploymentName = "gpt-4o-mini";
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public OnlineResearchService(AzureOpenAIClient openAIClient)
    {
        _client = openAIClient ?? throw new ArgumentNullException(nameof(openAIClient));
    }

    public async Task<IEnumerable<VectorResourceDto>> PerformOnlineResearchAsync(string query, int topK = 3)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException("Query cannot be empty", nameof(query));
        }

        try
        {
            var response = await GetOpenAIResponseAsync(query, topK);
            return await ParseResponseToVectorResourceDtos(response);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private async Task<string> GetOpenAIResponseAsync(string query, int topK)
    {
        var prompt = GeneratePrompt(query, topK);
        ChatCompletion response = await _client.GetChatClient(_deploymentName)
                    .CompleteChatAsync(prompt);

        return response.Content?.FirstOrDefault()?.Text ?? string.Empty;
    }

    private async Task<IEnumerable<VectorResourceDto>> ParseResponseToVectorResourceDtos(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
        {
            return new[] { CreateFallbackDto(response) };
        }

        try
        {
            // Clean the response by removing Markdown code block and escapes
            var cleanedResponse = response
                .Replace("```json", "")
                .Replace("```", "")
                .Replace("\\n", "")
                .Trim();

            var results = await Task.Run(() =>
                JsonSerializer.Deserialize<VectorResourceDto[]>(cleanedResponse, _jsonOptions));

            if (results?.Any() == true)
            {
                return results;
            }

            // Try parsing as single object if array fails
            var singleResult = await Task.Run(() =>
                JsonSerializer.Deserialize<VectorResourceDto>(cleanedResponse, _jsonOptions));

            return singleResult != null
                ? new[] { singleResult }
                : new[] { CreateFallbackDto(response) };
        }
        catch (JsonException ex)
        {
            return new[] { CreateFallbackDto(response) };
        }
    }

    private static string GeneratePrompt(string query, int topK)
    {
        return @$"
                You are an AI assistant. Given a user query, return an array of {topK} JSON objects with the following fields suitable for a vector database:

                [
                    {{
                        ""Id"": ""unique-id-123"",
                        ""Content"": ""First concise, factual answer here."",
                        ""Score"": 0.95,
                        ""Url"": ""https://relevant-source-1.com""
                    }},
                    {{
                        ""Id"": ""unique-id-456"",
                        ""Content"": ""Second concise, factual answer here."",
                        ""Score"": 0.85,
                        ""Url"": ""https://relevant-source-2.com""
                    }}
                ]

                User query: {query}

                Return exactly {topK} JSON objects in an array. Ensure each answer is unique and relevant.";
    }

    private static VectorResourceDto CreateFallbackDto(string content) => new()
    {
        Id = IdGeneratorUtil.GetNextId().ToString(),
        Content = content,
        Score = 0,
        Url = string.Empty
    };
}
