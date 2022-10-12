using Data.Abstractions;
using Data.Entities;
using Data.Models;

namespace Data.Providers;

public interface IJsonCustomersStorage : IJsonFileProvider<int, Customer>
{
}

public class JsonCustomersStorage : JsonFileProvider<int, Customer>, IJsonCustomersStorage
{
    private const string DefaultFilename = "customers.json";

    public JsonCustomersStorage(FileStorageConfiguration config)
        : base(config.Filename ?? DefaultFilename, config.IoRetryCount, config.IoRetryDelayMilliseconds)
    {
    }

    public JsonCustomersStorage(Action<FileStorageConfiguration> configFactory)
        : base(configFactory)
    {
    }
}
