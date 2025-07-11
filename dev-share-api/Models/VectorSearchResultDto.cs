namespace Models;

public class ResourceDto
{
    public string Id { get; set; }
    public string Url { get; set; }
    public string Content { get; set; }
    public float Score { get; set; }
}

public class InsightDto
{
    public string Id { get; set; }
    public string Url { get; set; }
    public string Content { get; set; }
    public string ResourceId { get; set; }
    public float Score { get; set; }
}
