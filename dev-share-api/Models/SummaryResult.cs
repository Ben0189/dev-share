using System.Text.Json.Serialization;

namespace Models;

public class SummaryResult
{
    [JsonPropertyName("summary")]
    public string Summary { get; set; }
    
    [JsonPropertyName("title")]
    public string Title { get; set; }
}