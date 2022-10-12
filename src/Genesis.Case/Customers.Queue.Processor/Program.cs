using System.Text;
using Customers.Queue.Processor;
using Customers.Queue.Processor.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Refit;

var factory = new ConnectionFactory {HostName = "rabbitmq", Port = 5672, UserName = "user", Password = "bitnami"};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(
    queue: "customer_create_requests",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (ch, ea) =>
{
    try
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("{0} | Received message: {1}", DateTimeOffset.UtcNow, message);
        Console.ResetColor();
    
        var createRequest = JsonConvert.DeserializeObject<CreateCustomerRequest>(message);
        if (createRequest is null)
        {
            return;
        }

#pragma warning disable CS4014
        RestService.For<ICustomersApi>("http://customers-api:80").CreateCustomerAsync(createRequest);
#pragma warning restore CS4014
    }
    catch (Exception e)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(e);
        Console.ResetColor();
    }
};

channel.BasicConsume(queue: "customer_create_requests",
    autoAck: true,
    consumer: consumer);

Console.WriteLine("Customers creation requests processor configured and ready to process messages");
while (true)
{
    // to prevent application close
}