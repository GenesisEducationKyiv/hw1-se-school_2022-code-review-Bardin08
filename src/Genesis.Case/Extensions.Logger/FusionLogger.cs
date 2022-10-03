using System.Text;
using Integration.RabbitMq.Abstractions;
using Integration.RabbitMq.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Extensions.Logger;

public class FusionLoggerFactory : ILoggerFactory
{
    private readonly IProducer _producer;

    public FusionLoggerFactory(IProducer producer)
    {
        _producer = producer;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new FusionLogger(_producer);
    }

    public void AddProvider(ILoggerProvider provider)
    {
    }

    public void Dispose()
    {
    }
}

public class FusionLogger : ILogger
{
    private const string CategoryName = "exchange_api_logs";
    private readonly IProducer _producer;

    public FusionLogger(IProducer producer)
    {
        _producer = producer;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        _producer.CreateQueue(new QueueModel
        {
            QueueName = CategoryName
        });

        var message = new LogEventAsMessage()
        {
            Level = logLevel, EventId = eventId.Id, Message = formatter(state, exception)
        };

        var amqpMessage = new MessageModel
        {
            RoutingKey = CategoryName,
            Data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message))
        };
        
        _producer.SendMessages(new[] {amqpMessage});
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return null!;
    }
}

public class LogEventAsMessage
{
    public LogLevel Level { get; set; }
    public int EventId { get; set; }
    public string Message { get; set; }
}