using Integrations.RabbitMq.Models;

namespace Integrations.RabbitMq.Abstractions;

public interface IAmqpProducer
{
    void CreateQueue(QueueModel queueDescriptor);
    void SendMessages(IEnumerable<MessageModel> messages);
}