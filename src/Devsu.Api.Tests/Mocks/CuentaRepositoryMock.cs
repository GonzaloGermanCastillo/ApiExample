using Devsu.Data;
using Devsu.Models;
using System.Collections.ObjectModel;

namespace Devsu.Api.Tests.Mocks;

internal class CuentaRepositoryMock : IRepository<Cuenta>
{
    private readonly IList<Cuenta> cuentas = new List<Cuenta>();

    public CuentaRepositoryMock(Cuenta cuenta)
    {
        cuentas.Add(cuenta);
    }

    public CuentaRepositoryMock(ReadOnlyCollection<Cuenta> cuentas)
    {
        this.cuentas = cuentas;
    }

    public Task<Cuenta> AddAsync(Cuenta entity)
    {
        return Task.FromResult(entity);
    }

    public IQueryable<Cuenta> AsQueryable()
    {
        return cuentas.AsQueryable();
    }

    public Task Delete(Cuenta entity)
    {
        var el = cuentas.FirstOrDefault(x => x.Id == entity.Id);
        if (el != null)
        {
            cuentas.Remove(el);
        }
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Cuenta>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Cuenta>>(cuentas);
    }

    public Task<Cuenta?> GetAsync<Guid>(Guid id) where Guid : notnull
    {
        return Task.FromResult<Cuenta?>(cuentas.FirstOrDefault(x => x.Id.Equals(id)));
    }

    public Task Update(Cuenta entity)
    {
        var el = cuentas.FirstOrDefault(x => x.Id == entity.Id);
        if (el != null)
        {
            cuentas.Remove(el);
            cuentas.Add(entity);
        }
        return Task.CompletedTask;
    }    
}
