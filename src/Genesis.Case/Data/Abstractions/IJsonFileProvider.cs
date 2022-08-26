namespace Data.Abstractions;

public interface IJsonFileProvider<in TKey, TEntity> : IDataProvider<TKey, TEntity>
{
}