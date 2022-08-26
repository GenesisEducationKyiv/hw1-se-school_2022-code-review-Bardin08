namespace Core.Models;

public class SendEmailResult
{
    public string? Email { get; set; }
    public bool IsSuccessful { get; set; }
    public string[]? Errors { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}