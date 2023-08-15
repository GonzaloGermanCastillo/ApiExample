using Devsu.Models;
using System.ComponentModel.DataAnnotations;

namespace Devsu.Api.Models;

public class ClienteViewModel
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = "";
    public string? Genero { get; set; }
    public int? Edad { get; set; }    
    public string? Identificacion { get; set; }    
    public string Direccion { get; set; } = "";
    public string Telefono { get; set; } = "";
    public bool Estado { get; set; } = true;

    public static ClienteViewModel From(Cliente cliente) => new()
    {
        Id = cliente.Id,
        Nombre = cliente.Nombre,
        Genero = cliente.Genero?.ToString() ?? "",
        Edad = cliente.Edad,    
        Direccion = cliente.Direccion,
        Estado = cliente.Estado,
        Identificacion = cliente.Identificacion,
        Telefono = cliente.Telefono
    };    
}

public class ClienteInputModel : IValidatableObject
{
    [Required(ErrorMessage = "Valor obligatorio")]
    [StringLength(50, ErrorMessage = "Largo máximo 50")]
    public string Nombre { get; set; } = "";    
    public string? Genero { get; set; }
    public int? Edad { get; set; }
    [StringLength(50, ErrorMessage = "Largo máximo 50")]
    public string? Identificacion { get; set; }
    [Required(ErrorMessage = "Valor obligatorio")]
    [StringLength(150, ErrorMessage = "Largo máximo 150")]
    public string Direccion { get; set; } = "";
    [Required(ErrorMessage = "Valor obligatorio")]
    [StringLength(15, ErrorMessage = "Largo máximo 15")]
    public string Telefono { get; set; } = "";
    [Required(ErrorMessage = "Valor obligatorio")]
    public int Contrasenia { get; set; }
 
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if(!string.IsNullOrEmpty(Genero) && Genero.ToLower() != "m" && Genero.ToLower() != "f")
        {
            yield return new ValidationResult("Género no válido", new[] { nameof(Genero) });
        }
    }
}
