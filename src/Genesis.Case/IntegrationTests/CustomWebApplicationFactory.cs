using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Providers;
using Integration.RabbitMq.Abstractions;
using Integration.RabbitMq.Models;
using Integration.RabbitMq.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Moq;

namespace IntegrationTests;

// ReSharper disable once ClassNeverInstantiated.Global
public class CustomWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup> where TStartup: class
{
    public Guid TestUId { get; set; } = Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // ReSharper disable once AsyncVoidLambda
        builder.ConfigureServices(async s =>
        {
            var emailStorageDescriptor = new ServiceDescriptor(
                typeof(IJsonEmailsStorage),
                new JsonEmailsStorage(cfg => cfg.Filename = $"emails-{TestUId}.json"));

            s.Replace(emailStorageDescriptor);

            var amqpProviderMock = new Mock<IAmqpProducer>();

            amqpProviderMock.SetupSequence(x => x.CreateQueue(It.IsAny<QueueModel>())).Pass();
            amqpProviderMock.SetupSequence(x => x.SendMessages(It.IsAny<IEnumerable<MessageModel>>())).Pass();

            var amqpProducerDescriptor = new ServiceDescriptor(typeof(IAmqpProducer), amqpProviderMock.Object);

            s.Replace(amqpProducerDescriptor);

            var serviceProvider = s.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            
            var emailsStorage = scopedServices.GetRequiredService<IJsonEmailsStorage>();
            var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

            try
            {
                // As email storage does not have any methods to check if it is initiated correctly,
                // to check that we can add a test record, and then remove it.
                // If something went wrong we can handle it at the catch block and log that emails storage initiated incorrectly.

                const string email = "integration-tests@gmail.com";
                await emailsStorage.CreateAsync(email);
                await emailsStorage.DeleteAsync(email);

                logger.LogInformation("[Integration Tests Server] Emails storage initiated correctly");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[Integration Tests Server] Emails storage initiated incorrectly");
            }
        });
    }

    public override async ValueTask DisposeAsync()
    {
        var scope = Services.CreateScope();
        var emailsStorage = scope.ServiceProvider.GetRequiredService<IJsonEmailsStorage>();

        var allEmails = await emailsStorage.ReadAllAsync(0, 0);

        foreach (var email in allEmails)
        {
            await emailsStorage.DeleteAsync(email);
        }

        await base.DisposeAsync();
    }
}