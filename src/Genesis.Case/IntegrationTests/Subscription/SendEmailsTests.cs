using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Api;
using Api.Models.Responses;
using Data.Providers;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace IntegrationTests.Subscription;

[Collection("Subscription")]
public class SendEmailsTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;
    private readonly IJsonEmailsStorage _emailsStorage;
    
    public SendEmailsTests(CustomWebApplicationFactory<Program> factory)
    {
        factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped<IJsonEmailsStorage>(_ => new JsonEmailsStorage($"emails_{Guid.NewGuid():N}"));
            });
        });
        
        _httpClient = factory.CreateClient();

        var scope = factory.Services.CreateScope();
        _emailsStorage = scope.ServiceProvider.GetService<IJsonEmailsStorage>()!;
    }

    [Fact]
    public async Task SendEmails()
    {
        var expectedResponse = new SendEmailsResponse
        {
            TotalSubscribers = 1,
            SuccessfullyNotified = 1,
            Failed = new List<string>()
        };
        
        const string emailTemplate = "integration-tests{0}@gmail.com";
        var subscriber = string.Format(emailTemplate, Guid.NewGuid());

        await _emailsStorage.CreateAsync(subscriber);

        var response = await _httpClient.PostAsync("/sendEmails", new StringContent(""));
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var responseModel = JsonConvert.DeserializeObject<SendEmailsResponse>(responseString)!;

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(expectedResponse.TotalSubscribers, responseModel.TotalSubscribers);
        Assert.Equal(expectedResponse.SuccessfullyNotified, responseModel.SuccessfullyNotified);
        Assert.Equal(expectedResponse.Failed, responseModel.Failed);
    }
}