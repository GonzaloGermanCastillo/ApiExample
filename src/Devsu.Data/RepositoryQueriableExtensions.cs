using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Devsu.Data;

public static class RepositoryQueriableExtensions
{
    public static async Task<IReadOnlyCollection<TEntity>> Results<TEntity>(this IQueryable<TEntity> source, CancellationToken cancellationToken = default)
            => source.Provider is Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryProvider
            ? await source.ToListAsync(cancellationToken)
            : source.ToList();

    public static async Task<TEntity?> FirstOrDefaultResult<TEntity>(this IQueryable<TEntity> source, CancellationToken cancellationToken = default)
        => source.Provider is Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryProvider
        ? await source.FirstOrDefaultAsync(cancellationToken)
        : source.FirstOrDefault();

    public static async Task<TEntity?> FirstOrDefaultResult<TEntity>(this IQueryable<TEntity> source, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => source.Provider is Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryProvider
        ? await source.FirstOrDefaultAsync(predicate, cancellationToken)
        : source.FirstOrDefault(predicate);

    public static async Task<int> CountResults<TEntity>(this IQueryable<TEntity> source, CancellationToken cancellationToken = default)
        => source.Provider is Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryProvider
        ? await source.CountAsync(cancellationToken)
        : source.Count();

    public static async Task<int> CountResults<TEntity>(this IQueryable<TEntity> source, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => source.Provider is Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryProvider
        ? await source.CountAsync(predicate, cancellationToken)
        : source.Count(predicate);  

    public static async Task<decimal> SumResults<TEntity>(this IQueryable<TEntity> source, Expression<Func<TEntity, decimal>> predicate, CancellationToken cancellationToken = default)
        => source.Provider is Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryProvider
        ? await source.SumAsync(predicate, cancellationToken)
        : source.Sum(predicate);

    public static async Task<double> SumResults<TEntity>(this IQueryable<TEntity> source, Expression<Func<TEntity, double>> predicate, CancellationToken cancellationToken = default)
        => source.Provider is Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryProvider
        ? await source.SumAsync(predicate, cancellationToken)
        : source.Sum(predicate);
}
