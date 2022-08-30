namespace Data.Abstractions;

public interface IDataProvider<in TKey, TEntity>
{
    Task<TEntity> CreateAsync(TEntity entity);
    Task<TEntity?> ReadAsync(TKey id);
    Task<IEnumerable<TEntity>> ReadAllAsync(int offset, int pageSize);
    Task<TEntity> DeleteAsync(TEntity entity);
}