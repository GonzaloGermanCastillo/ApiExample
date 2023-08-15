using Devsu.Data;
using Devsu.Models;

namespace Devsu.Api.Tests.Mocks;

internal class MovimientosRepositoryMock : IRepository<Movimiento>
{
    private readonly IList<Movimiento> movimientos = new List<Movimiento>();

    public MovimientosRepositoryMock()
    {        
    }

    public MovimientosRepositoryMock(Movimiento movimiento)
    {
        movimientos.Add(movimiento);
    }

    public MovimientosRepositoryMock(IList<Movimiento> movimientos)
    {
        this.movimientos = movimientos;
    }

    public Task<Movimiento> AddAsync(Movimiento entity)
    {
        movimientos.Add(entity);
        return Task.FromResult(entity);
    }

    public IQueryable<Movimiento> AsQueryable()
    {
        return movimientos.AsQueryable();
    }

    public Task Delete(Movimiento entity)
    {
        var el = movimientos.FirstOrDefault(x => x.Id == entity.Id);
        if (el != null)
        {
            movimientos.Remove(el);
        }
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Movimiento>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Movimiento>>(movimientos);
    }

    public Task<Movimiento?> GetAsync<Guid>(Guid id) where Guid : notnull
    {
        return Task.FromResult<Movimiento?>(movimientos.FirstOrDefault(x => x.Id.Equals(id)));
    }

    public Task Update(Movimiento entity)
    {
        var el = movimientos.FirstOrDefault(x => x.Id == entity.Id);
        if (el != null)
        {
            movimientos.Remove(el);
            movimientos.Add(entity);
        }
        return Task.CompletedTask;
    }    
}
