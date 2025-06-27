namespace Models;

public class HandlerResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public static HandlerResult Success() => new() { IsSuccess = true};
    public static HandlerResult Fail(string message) => new() { IsSuccess = false, ErrorMessage = message };
}