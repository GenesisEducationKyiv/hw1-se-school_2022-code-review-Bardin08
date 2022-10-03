using Integration.RabbitMq.Models;

namespace Integration.RabbitMq.Abstractions;

public interface IProducer
{
    void CreateQueue(QueueModel queueDescriptor);
    void SendMessages(IEnumerable<MessageModel> messages);
}