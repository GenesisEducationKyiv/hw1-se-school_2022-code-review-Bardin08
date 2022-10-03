using Integrations.RabbitMq.Abstractions;
using Integrations.RabbitMq.Models;
using RabbitMQ.Client;

namespace Integrations.RabbitMq.Services;

public class AmqpProducer : IAmqpProducer
{
    private readonly RabbitConfiguration _rabbitConfiguration;
    
    public AmqpProducer(RabbitConfiguration rabbitConfiguration)
    {
        _rabbitConfiguration = rabbitConfiguration;
    }

    public void CreateQueue(QueueModel queueDescriptor)
    {
        var factory = new ConnectionFactory
        {
            HostName = _rabbitConfiguration.HostName,
            Port = _rabbitConfiguration.Port,
            UserName = _rabbitConfiguration.UserName,
            Password = _rabbitConfiguration.Password
        };

        using var connection = factory.CreateConnection();
        var channel = connection.CreateModel();
        
        channel.QueueDeclare(
            queueDescriptor.QueueName,
            queueDescriptor.IsDurable,
            queueDescriptor.IsExclusive,
            queueDescriptor.IsAutoDelete,
            queueDescriptor.Arguments);
    }

    public void SendMessages(IEnumerable<MessageModel> messages)
    {
        var factory = new ConnectionFactory
        {
            HostName = _rabbitConfiguration.HostName,
            Port = _rabbitConfiguration.Port,
            UserName = _rabbitConfiguration.UserName,
            Password = _rabbitConfiguration.Password
        };

        using var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        foreach (var message in messages)
        {
            channel.BasicPublish(message.Exchange, message.RoutingKey, message.BasicProperties, message.Data);
        }
    }
}