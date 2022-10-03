namespace Integration.RabbitMq.Models;

public class QueueModel
{
    public string QueueName { get; set; } = null!;
    public bool IsDurable { get; set; }
    public bool IsExclusive { get; set; }
    public bool IsAutoDelete { get; set; }
    public Dictionary<string, object> Arguments { get; set; } = null!;
}