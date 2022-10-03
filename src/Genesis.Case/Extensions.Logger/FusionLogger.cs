using System.Text;
using Integrations.RabbitMq.Abstractions;
using Integrations.RabbitMq.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Extensions.Logger;

public class FusionLoggerFactory : ILoggerFactory
{
    private readonly IAmqpProducer _amqpProducer;

    public FusionLoggerFactory(IAmqpProducer amqpProducer)
    {
        _amqpProducer = amqpProducer;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new FusionLogger(_amqpProducer);
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
    private readonly IAmqpProducer _amqpProducer;

    public FusionLogger(IAmqpProducer amqpProducer)
    {
        _amqpProducer = amqpProducer;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        _amqpProducer.CreateQueue(new QueueModel
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
        
        _amqpProducer.SendMessages(new[] {amqpMessage});
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