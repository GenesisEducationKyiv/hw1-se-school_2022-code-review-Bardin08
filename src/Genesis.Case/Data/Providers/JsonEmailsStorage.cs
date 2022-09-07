using Data.Abstractions;

namespace Data.Providers;

public interface IJsonEmailsStorage : IJsonFileProvider<string, string>
{
} 

public class JsonEmailsStorage : JsonFileProvider<string, string>, IJsonEmailsStorage
{
    private const string DefaultFilename = "emails.json";

    public JsonEmailsStorage(string filename = DefaultFilename) : base(DefaultFilename)
    {
    }

    public new async Task<string?> ReadAsync(string email)
    {
        var allEmails = await ReadAllAsync();
        return allEmails.FirstOrDefault(x => x!.Equals(email), null);
    }
}