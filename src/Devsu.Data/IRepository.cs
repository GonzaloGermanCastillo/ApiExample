namespace Devsu.Data;

public interface IRepository<T> where T : class
{
    Task<T?> GetAsync<TId>(TId id) where TId : notnull;
    Task<T> AddAsync(T entity);    
    Task Update(T entity);   
    Task Delete(T entity);
    Task<IEnumerable<T>> GetAllAsync();
    IQueryable<T> AsQueryable();
}
