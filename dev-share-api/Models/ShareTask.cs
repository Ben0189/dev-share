namespace Models;

public class ShareTask
{
    public string TaskId { get; set; }
    public string Url { get; set; }
    public string Status { get; set; } = "pending"; // pending | success | failed
    public string? Message { get; set; }
}