using RabbitMQ.Client;

namespace Integrations.RabbitMq.Models;

public class MessageModel
{
    public string RoutingKey { get; set; } = "default";
    public string Exchange { get; set; } = string.Empty;
    public IBasicProperties BasicProperties { get; set; } = null!;
    public ReadOnlyMemory<byte> Data { get; set; } = null!;
}