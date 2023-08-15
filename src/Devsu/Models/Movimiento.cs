namespace Devsu.Models;

public class Movimiento
{
    public const double MaximoExtraccion = 1000;
    public Guid Id { get; set; }
    public DateTime Fecha { get; set; }
    public TipoMovimiento TipoMovimiento { get; set; }
    public double Valor { get; set; }
    public double Saldo { get; set; }
    public virtual Cuenta Cuenta { get; set; } = new();
}

public enum TipoMovimiento
{
    Credito = 0,
    Debito = 1,
}
