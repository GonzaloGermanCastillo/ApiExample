using Devsu.Data;
using Devsu.Models;
using System.Collections.ObjectModel;

namespace Devsu.Api.Tests.Mocks;

internal class ClienteRepositoryMock : IRepository<Cliente>
{
    private readonly IList<Cliente> cuentas = new List<Cliente>();

    public ClienteRepositoryMock(Cliente cuenta)
    {
        cuentas.Add(cuenta);
    }

    public ClienteRepositoryMock(ReadOnlyCollection<Cliente> cuentas)
    {
        this.cuentas = cuentas;
    }

    public Task<Cliente> AddAsync(Cliente entity)
    {
        return Task.FromResult(entity);
    }

    public IQueryable<Cliente> AsQueryable()
    {
        return cuentas.AsQueryable();
    }

    public Task Delete(Cliente entity)
    {
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Cliente>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Cliente>>(cuentas);
    }

    public Task<Cliente?> GetAsync<TId>(TId id) where TId : notnull
    {
        return Task.FromResult(cuentas.FirstOrDefault());
    }

    public Task Update(Cliente entity)
    {
        return Task.CompletedTask;
    }
}
