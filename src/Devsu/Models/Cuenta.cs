namespace Devsu.Models;

public class Cuenta
{
    public Guid Id { get; set; }
    public int Numero { get; set; }
    public TipoCuenta TipoCuenta { get; set; }
    public double SaldoInicial { get; set; }
    public bool Estado { get; set; } = true;
    public virtual Cliente Cliente { get; set; } = new();
}

public enum TipoCuenta
{
    Ahorros = 0,
    Corriente = 1,
}
