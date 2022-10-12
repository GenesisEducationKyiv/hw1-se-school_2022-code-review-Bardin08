using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Api.Consumers;

/// <summary>
/// </summary>
public class CustomersQueuesConsumer : BackgroundService
{
    private readonly IConnectionFactory _connectionFactory;

    /// <summary>
    /// </summary>
    public CustomersQueuesConsumer()
    {
        _connectionFactory = new ConnectionFactory
        {
            HostName = "rabbitmq",
            Port = 5672,
            UserName = "user",
            Password = "bitnami"
        };
    }


    /// <summary>
    /// </summary>
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task.Run(() => DeclareQueueConsumer("created_customers", GetCreatedUsersEventHandler()), stoppingToken);
        Task.Run(() => DeclareQueueConsumer("failed_customers", GetFailedUsersEventHandler()), stoppingToken);
        
        return Task.CompletedTask;
    }

    private void DeclareQueueConsumer(string queueName, EventHandler<BasicDeliverEventArgs> eventHandler)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += eventHandler; 
        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        
        while (true)
        {
            // to prevent application close
        }

        // ReSharper disable once FunctionNeverReturns
    }

    private static EventHandler<BasicDeliverEventArgs> GetCreatedUsersEventHandler()
    {
        return (_, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("{0} | Customer created: {1}", DateTimeOffset.UtcNow, message);
                Console.ResetColor();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
                Console.ResetColor();
            }
        };
    }

    private static EventHandler<BasicDeliverEventArgs> GetFailedUsersEventHandler()
    {
        return (_, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("{0} | Customer wasn't created: {1}", DateTimeOffset.UtcNow, message);
                Console.ResetColor();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
                Console.ResetColor();
            }
        };
    }
}