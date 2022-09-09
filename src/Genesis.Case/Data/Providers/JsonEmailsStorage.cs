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

    /// <summary>
    /// Returns first email from the list, that is similar to the given email.
    /// </summary>
    /// <param name="email"> Email that we are searching at the storage. </param>
    /// <remarks>
    /// This method brakes the LSP, because emails storage is build like a list of string, without an ID for each email.
    /// And that's why we can't use the base interface of this method from <see cref="IDataProvider{TKey,TEntity}.ReadAsync"/>.
    /// </remarks>
    /// <returns> Email from the file storage </returns>
    public new async Task<string?> ReadAsync(string email)
    {
        var allEmails = await ReadAllAsync();
        return allEmails.FirstOrDefault(x => x!.Equals(email), null);
    }
}