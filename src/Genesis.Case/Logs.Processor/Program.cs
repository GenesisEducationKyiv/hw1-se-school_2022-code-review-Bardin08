using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory {HostName = "localhost", Port = 5672, UserName = "user", Password = "bitnami"};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(
    queue: "exchange_api_logs",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (ch, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    var log = JsonConvert.DeserializeObject<LogEventAsMessage>(message);

    if (log is {Level: not LogLevel.Error})
    {
        return;
    }

    Console.ForegroundColor = log.Level switch
    {
        LogLevel.Error => ConsoleColor.Red,
        _ => Console.ForegroundColor
    };

    Console.WriteLine(" [{0}] Received {1}", DateTime.UtcNow.ToLongTimeString(), message);
    Console.ResetColor();
};
channel.BasicConsume(queue: "exchange_api_logs",
    autoAck: true,
    consumer: consumer);


Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();

public class LogEventAsMessage
{
    public LogLevel Level { get; set; }
    public int EventId { get; set; }
    public string Message { get; set; }
}