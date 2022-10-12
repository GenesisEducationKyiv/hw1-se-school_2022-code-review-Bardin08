namespace Data.Abstractions;

public interface IJsonFileProvider<in TKey, TEntity> : IGenericRepository<TKey, TEntity>
{
}