namespace Models;

public class ResourceDto
{
    public long ResourceId { get; set; }
    public required string Url { get; set; }
    public string? NormalizeUrl { get; set; }
    public required string Content { get; set; }
    public List<UserInsightDto>? UserInsights  { get; set; }
}