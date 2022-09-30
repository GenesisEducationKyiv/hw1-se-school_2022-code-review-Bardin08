namespace Integrations.Notifications.Emails.Models;

public class EmailServiceConfiguration
{
    public string SmtpHost { get; set; } = null!;
    public int Port { get; set; }
    public string SenderName { get; set; } = null!;
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
}