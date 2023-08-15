using Devsu.Models;
using System.ComponentModel.DataAnnotations;

namespace Devsu.Api.Models;

public class MovimientoViewModel
{
    public Guid Id { get; set; }
    public DateTime Fecha { get; set; }
    public string Cliente { get; set; } = "";
    public int CuentaNumero { get; set; }
    public string TipoCuenta { get; set; } = "";
    public string TipoMovimiento { get; set; } = "";
    public double Valor { get; set; }

    public static MovimientoViewModel From(Movimiento movimiento) => new()
    {
        Id = movimiento.Id,
        Fecha = movimiento.Fecha,
        Cliente = movimiento.Cuenta.Cliente.Nombre,
        TipoCuenta = movimiento.Cuenta.TipoCuenta.ToString(),
        CuentaNumero = movimiento.Cuenta.Numero,
        Valor = movimiento.Valor
    };
}

public class MovimientoInputModel : IValidatableObject
{
    [Required(ErrorMessage = "Valor obligatorio")]
    public string Cliente { get; set; } = "";
    [Required(ErrorMessage = "Valor obligatorio")]
    public int CuentaNumero { get; set; }
    [Required(ErrorMessage = "Valor obligatorio")]
    public string TipoMovimiento { get; set; } = "";
    [Required(ErrorMessage = "Valor obligatorio")]
    [Range(0, double.MaxValue, ErrorMessage = "El valor sólo puede ser positivo")]
    public double Valor { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if(TipoMovimiento.ToLower() != "d" && TipoMovimiento.ToLower() != "c" &&
            TipoMovimiento.ToLower().Replace("é", "e") != "debito" && TipoMovimiento.ToLower().Replace("é", "e") != "credito")
        {
            yield return new ValidationResult("Tipo de movimiento no válido", new[] { nameof(TipoCuenta) });
        }
    }
}
