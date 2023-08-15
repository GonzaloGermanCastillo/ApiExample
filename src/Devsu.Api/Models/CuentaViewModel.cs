using Devsu.Models;
using System.ComponentModel.DataAnnotations;

namespace Devsu.Api.Models;

public class CuentaViewModel
{
    public Guid Id { get; set; }
    public int Numero { get; set; }    
    public string TipoCuenta { get; set; } = "";
    public double SaldoInicial { get; set; }
    public bool Estado { get; set; }
    public Guid ClienteId { get; set; }
    public string Cliente { get; set; } = "";

    public static CuentaViewModel From(Cuenta cuenta) => new()
    {
        Cliente = cuenta.Cliente.Nombre,
        ClienteId = cuenta.Cliente.Id,
        Numero = cuenta.Numero,
        Estado = cuenta.Estado,
        Id = cuenta.Id,
        SaldoInicial = cuenta.SaldoInicial,
        TipoCuenta = cuenta.TipoCuenta.ToString(),    
    };
}

public class CuentaInputModel : IValidatableObject
{
    [Required(ErrorMessage = "Valor obligatorio")]
    public int Numero { get; set; }
    [Required(ErrorMessage = "Valor obligatorio")]
    public string TipoCuenta { get; set; } = "";
    [Range(0, double.MaxValue, ErrorMessage = "El saldo inicial sólo puede ser positivo")]
    public double SaldoInicial { get; set; } = 0;
    [Required(ErrorMessage = "Valor obligatorio")]
    public string Cliente { get; set; } = "";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var tipo = TipoCuenta.ToLower();
        if(tipo != "ahorros" && tipo != "corriente")
        {
            yield return new ValidationResult("Tipo de cuenta no válido", new[] { nameof(TipoCuenta) });
        }
    }
}
