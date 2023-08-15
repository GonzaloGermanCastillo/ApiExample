using Microsoft.EntityFrameworkCore;

namespace Devsu.Data;
internal sealed class Repository<T> : IRepository<T> where T : class
{
    private readonly DevsuContext dbContext;
    
    public Repository(DevsuContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<T> AddAsync(T entity)
    {
        await dbContext.Set<T>().AddAsync(entity);
        await dbContext.SaveChangesAsync();
        return entity;
    }    

    public async Task Update(T entity)
    {
        dbContext.Set<T>().Update(entity);
        await dbContext.SaveChangesAsync();
    }
    
    public async Task Delete(T entity)
    {
        dbContext.Set<T>().Remove(entity);
        await dbContext.SaveChangesAsync();
    }
    
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await dbContext.Set<T>().ToListAsync();
    }

    public async Task<T?> GetAsync<TId>(TId id) where TId : notnull
    {
        return await dbContext.Set<T>().FindAsync(new object[1] { id });
    }

    public IQueryable<T> AsQueryable() => dbContext.Set<T>();
}