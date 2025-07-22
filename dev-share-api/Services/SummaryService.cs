using System.Text.Json;
using Azure.AI.OpenAI;
using Models;
using OpenAI.Chat;

namespace Services;

public interface ISummaryService
{
    Task<SummaryResult> SummarizeAsync(string content);
}

public class SummaryService : ISummaryService
{
    private readonly AzureOpenAIClient _client;
    private const string _deploymentName = "gpt-4o-mini";

    public SummaryService(AzureOpenAIClient openAIClient)
    {
        _client = openAIClient;
    }

    public async Task<SummaryResult> SummarizeAsync(string content)
    {
       
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage($@"
            You are a helpful summarization assistant.
            Your task is to:
            1. Summarize the article in no more than 100 words.
            2. Extract or infer a clear, appropriate title, no more than 12 words.
            
            Always call the `generate_summary` function with your result in JSON:
            {{
              ""summary"": string,
              ""title"": string
            }}

            Guidelines:
            - Avoid fabricating content. Be brief and accurate. Do not include any explanation.
            - If the article lacks detail, summarize what’s available.
            - Never return plain text. Always return structured JSON using the `generate_summary` function.
        "),
            new UserChatMessage(content)
        };
        
        var tool = CreateGenerateSummaryTool();

        return await CallToolAndDeserializeAsync<SummaryResult>(
            toolFunctionName: "generate_summary",
            messages: messages,
            tool: tool
        );
        
    }
    

    private ChatTool CreateGenerateSummaryTool()
    {
        return ChatTool.CreateFunctionTool(
            functionName: "generate_summary",
            functionDescription: "Generates a short summary and a suitable title for the provided article.",
            functionParameters: BinaryData.FromObjectAsJson(new
            {
                type = "object",
                properties = new
                {
                    summary = new
                    {
                        type = "string",
                        description = "A concise summary of the article, no more than 100 words."
                    },
                    title = new
                    {
                        type = "string",
                        description = "A short, clear, and appropriate title that reflects the article's content, no more than 12 words."
                    }
                },
                required = new[] { "summary", "title" }
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
        ChatCompletion response = await client.CompleteChatAsync(
            messages: messages,
            options
        );

        var toolCall = response.ToolCalls.FirstOrDefault(tc => tc.FunctionName == toolFunctionName);
        if (toolCall == null)
        {
            throw new InvalidOperationException("No function call response found.");
        }

        var json = toolCall.FunctionArguments.ToString();
        var result = JsonSerializer.Deserialize<T>(json);
        if (result == null)
        {
            throw new InvalidOperationException("Deserialization failed.");
        }

        return result;
    }
}
