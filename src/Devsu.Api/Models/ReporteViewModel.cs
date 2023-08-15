using Devsu.Models;

namespace Devsu.Api.Models;

public class ReporteViewModel
{
    public DateTime Fecha { get; set; }
    public string Cliente { get; set; } = "";
    public int NumeroCuenta { get; set; }
    public string TipoCuenta { get; set; } = "";
    public double SaldoInicial { get; set; }
    public bool Estado { get; set; }
    public double Movimiento { get; set; }
    public double SaldoDisponible { get; set; }

    public static ReporteViewModel From(Movimiento movimiento) => new()
    {
        Fecha = movimiento.Fecha,
        Cliente = movimiento.Cuenta.Cliente.Nombre,
        NumeroCuenta = movimiento.Cuenta.Numero,
        TipoCuenta = movimiento.Cuenta.TipoCuenta.ToString(),
        SaldoInicial = movimiento.Cuenta.SaldoInicial,
        Estado = movimiento.Cuenta.Estado,
        Movimiento = movimiento.Valor,
        SaldoDisponible = movimiento.Saldo,
    };
}
