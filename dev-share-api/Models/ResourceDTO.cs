using Entities;

namespace Models;

public class ResourceDTO
{
    public long ResourceId { get; set; }
    public string Url { get; set; }
    public string NormalizeUrl { get; set; }
    public string Content { get; set; }
    public List<UserInsightDTO> UserInsights  { get; set; }
}