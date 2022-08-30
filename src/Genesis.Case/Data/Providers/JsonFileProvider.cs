using System.Text;
using Data.Abstractions;
using Data.Exceptions;
using Newtonsoft.Json;

namespace Data.Providers;

public abstract class JsonFileProvider<TKey, TEntity> : IJsonFileProvider<TKey, TEntity>
{
    private const string DataFolder = "__data";
    private readonly string _dataFilePath;

    public JsonFileProvider(string fileName)
    {
        string dataFilesDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), DataFolder);
        if (!Directory.Exists(dataFilesDirectoryPath))
        {
            Directory.CreateDirectory(dataFilesDirectoryPath);
        }

        _dataFilePath = Path.Combine(dataFilesDirectoryPath, fileName);
    }

    public async Task<TEntity> CreateAsync(TEntity entity)
    {
        var allRecords = (await ReadAllAsync()).ToList();

        if (allRecords.Contains(entity))
        {
            throw new RecordAlreadyExistsException(JsonConvert.SerializeObject(entity));
        }

        allRecords.Add(entity);
        await WriteNewAsync(allRecords);
        return entity;
    }

    public async Task<TEntity?> ReadAsync(TKey id)
    {
        var allRecords = (await ReadAllAsync()).ToList();

        var keyEqualsPredicate = new Func<TEntity, bool>(x =>
        {
            var idPropertyInfo = x?.GetType().GetProperty("Id");
            var isContainsId = idPropertyInfo != null;
            return isContainsId && idPropertyInfo!.GetValue(idPropertyInfo)!.Equals(id);
        });

        if (allRecords.FirstOrDefault(x => keyEqualsPredicate(x)) is { } entity)
        {
            return entity;
        }

        return default;
    }

    public async Task<IEnumerable<TEntity>> ReadAllAsync(int offset = 0, int pageSize = 0)
    {
        if (!File.Exists(_dataFilePath))
        {
            File.Create(_dataFilePath).Close();
        }

        var json = await File.ReadAllTextAsync(_dataFilePath, Encoding.UTF8);
        if (json is {Length: < 2})
        {
            json = "[]";
        }

        var records = JsonConvert.DeserializeObject<List<TEntity>>(json) ?? new List<TEntity>();

        return pageSize switch
        {
            0 => records,
            > 0 => records.Skip(offset).Take(pageSize),
            _ => Enumerable.Empty<TEntity>()
        };
    }

    public async Task<TEntity> DeleteAsync(TEntity entity)
    {
        var records = (await ReadAllAsync()).ToList();

        records.Remove(entity);
        await WriteNewAsync(records);

        return entity;
    }

    private async Task WriteNewAsync(List<TEntity> entities)
    {
        if (!File.Exists(_dataFilePath))
        {
            File.Create(_dataFilePath).Close();
        }

        var json = JsonConvert.SerializeObject(entities);
        await File.WriteAllTextAsync(_dataFilePath, json, Encoding.UTF8);
    }
}