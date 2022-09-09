using Data.Abstractions;
using Data.Models;

namespace Data.Providers;

public interface IJsonEmailsStorage : IJsonFileProvider<string, string>
{
} 

public class JsonEmailsStorage : JsonFileProvider<string, string>, IJsonEmailsStorage
{
    private const string DefaultFilename = "emails.json";

    public JsonEmailsStorage(FileStorageConfiguration config)
        : base(config.Filename ?? DefaultFilename, config.IoRetryCount, config.IoRetryDelayMilliseconds)
    {
    }

    public JsonEmailsStorage(Action<FileStorageConfiguration> configFactory)
        : base(configFactory)
    {
    }

    public new async Task<string?> ReadAsync(string email)
    {
        var allEmails = await ReadAllAsync();
        return allEmails.FirstOrDefault(x => x!.Equals(email), null);
    }
}