namespace Models;

public class SearchRequest
{
    public required string Text { get; set; }
    public int TopRelatives { get; set; } = 5;
}