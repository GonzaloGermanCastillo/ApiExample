namespace Devsu.Models;

public abstract class Persona
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = "";
    public Genero? Genero { get; set; }
    public int? Edad { get; set; }
    public string? Identificacion { get; set; }
    public string Direccion { get; set; } = "";
    public string Telefono { get; set; } = "";
}

public enum Genero
{
    Masculino = 0,
    Femenino = 1,
}
