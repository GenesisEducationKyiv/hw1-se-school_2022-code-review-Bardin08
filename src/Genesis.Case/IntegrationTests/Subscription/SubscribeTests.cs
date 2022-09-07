using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Api;
using Data.Providers;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IntegrationTests.Subscription;

public class SubscribeTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;

    public SubscribeTests(CustomWebApplicationFactory<Program> factory)
    {
        _httpClient = factory.CreateDefaultClient();
    }

    [Fact]
    public async Task Post_Subscribe_ReturnsOk()
    {
        const string emailTemplate = "integration-tests{0}@gmail.com";
        var email = string.Format(emailTemplate, Guid.NewGuid());
        
        var formData = new MultipartFormDataContent();
        formData.Add(new StringContent(email));

        var response = await _httpClient.PostAsync("/subscribe", formData);
        response.EnsureSuccessStatusCode();
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);        
        Assert.Equal("\"Email added successfully!\"", await response.Content.ReadAsStringAsync());        
    }
}