using System.Net;
using Polly;
using Polly.Extensions.Http;

namespace Integrations.Crypto;

public static class HttpRetryPolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(x => x.StatusCode == HttpStatusCode.NotFound)
            .OrResult(x => x.StatusCode == HttpStatusCode.BadGateway)
            .WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(300));
    }
}