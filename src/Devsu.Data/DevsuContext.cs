using Devsu.Models;
using Microsoft.EntityFrameworkCore;

namespace Devsu.Data;

public class DevsuContext : DbContext
{
    public DevsuContext(DbContextOptions<DevsuContext> options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>().ToTable("Clientes");
        modelBuilder.Entity<Cuenta>().Navigation(e => e.Cliente).AutoInclude();
        modelBuilder.Entity<Movimiento>().Navigation(e => e.Cuenta).AutoInclude();
    }

    public DbSet<Persona> Personas { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Cuenta> Cuentas { get; set; }
    public DbSet<Movimiento> Movimientos { get; set; }
}
