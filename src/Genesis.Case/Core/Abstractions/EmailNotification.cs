namespace Core.Abstractions;

public class EmailNotification
{
    public string? To { get; set; }
    public string? Subject { get; set; }
    public string? Message { get; set; }
}