using Devsu.Api.Models;
using Devsu.Data;
using Devsu.Models;
using Microsoft.AspNetCore.Mvc;

namespace Devsu.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : BaseController
{
    private readonly IRepository<Cliente> clienteRepository;
    private readonly IRepository<Cuenta> cuentaRepository;

    public ClientesController(IRepository<Cliente> clienteRepository, IRepository<Cuenta> cuentaRepository)
    {
        this.clienteRepository = clienteRepository ?? throw new ArgumentNullException(nameof(clienteRepository));
        this.cuentaRepository = cuentaRepository ?? throw new ArgumentNullException(nameof(cuentaRepository));
    }

    [HttpGet("")]
    public async Task<IActionResult> Get()
    {
        var clientes = await clienteRepository.GetAllAsync();
        var models = clientes.Select(x => ClienteViewModel.From(x));
        return Ok(models);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute]Guid id)
    {
        if(id == Guid.Empty)
        {
            ModelState.AddModelError(nameof(id), "Id no válido.");
            return ValidationProblem(ModelState);
        }
        var cliente = await clienteRepository.GetAsync(id);
        if(cliente is null)
        {            
            return NotFoundProblem("Cliente no existe");
        }
        return Ok(ClienteViewModel.From(cliente));
    }
    
    [HttpPost]    
    public async Task<IActionResult> Create([FromBody]ClienteInputModel model)
    {
        if(!ModelState.IsValid)
        {       
            return ValidationProblem(ModelState);
        }

        var exists = await clienteRepository.AsQueryable().FirstOrDefaultResult(x => x.Nombre == model.Nombre);

        if(exists is not null)
        {
            ModelState.AddModelError(nameof(ClienteInputModel.Nombre), "El cliente ya existe.");
            return ConflictProblem(ModelState);
        }

        var genero = !string.IsNullOrEmpty(model.Genero) 
            ? Enum.GetValues(typeof(Genero))
                  .Cast<Genero>()
                  .FirstOrDefault(x => x.ToString().ToLower() == model.Genero)
            : (Genero?) null;

        var id = Guid.NewGuid();

        var nuevoCliente = new Cliente
        {
            Contrasenia = model.Contrasenia,
            Direccion = model.Direccion,
            Edad = model.Edad,
            Id = id,
            Genero = genero,
            Identificacion = model.Identificacion,
            Nombre = model.Nombre,
            Telefono = model.Telefono
        };
        await clienteRepository.AddAsync(nuevoCliente);

        return Created(Url.RouteUrl("", new { controller = "clientes", action = "", id = id}) ?? "", ClienteViewModel.From(nuevoCliente));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] ClienteInputModel model)
    {
        if (id == Guid.Empty)
        {
            ModelState.AddModelError(nameof(id), "Id no válido.");
            return ValidationProblem(ModelState);
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var cliente = await clienteRepository.GetAsync(id);

        if (cliente is null)
        {
            return NotFoundProblem("Cliente no existe");
        }

        var exists = await clienteRepository.AsQueryable().FirstOrDefaultResult(x => x.Nombre == model.Nombre && x.Id != cliente.Id);

        if (exists is not null)
        {
            ModelState.AddModelError(nameof(ClienteInputModel.Nombre), "El cliente ya existe.");
            return ConflictProblem(ModelState);
        }

        var genero = !string.IsNullOrEmpty(model.Genero)
            ? Enum.GetValues(typeof(Genero))
                  .Cast<Genero>()
                  .FirstOrDefault(x => x.ToString().ToLower() == model.Genero)
            : (Genero?)null;

        cliente.Contrasenia = model.Contrasenia;
        cliente.Direccion = model.Direccion;
        cliente.Edad = model.Edad;
        cliente.Genero = genero;
        cliente.Identificacion = model.Identificacion;
        cliente.Nombre = model.Nombre;
        cliente.Telefono = model.Telefono;

        await clienteRepository.Update(cliente);

        return Ok(ClienteViewModel.From(cliente));
    }

    [HttpPatch("{id}/Desactivar")]
    public async Task<IActionResult> Desactivar([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
        {
            ModelState.AddModelError(nameof(id), "Id no válido.");
            return ValidationProblem(ModelState);
        }
        var cliente = await clienteRepository.GetAsync(id);

        if (cliente is null)
        {
            return NotFoundProblem("Cliente no existe");
        }

        cliente.Estado = false;
        await clienteRepository.Update(cliente);

        return NoContent();
    }

    [HttpPatch("{id}/Activar")]
    public async Task<IActionResult> Activar([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
        {
            ModelState.AddModelError(nameof(id), "Id no válido.");
            return ValidationProblem(ModelState);
        }
        var cliente = await clienteRepository.GetAsync(id);

        if (cliente is null)
        {
            return NotFoundProblem("Cliente no existe");
        }

        cliente.Estado = true;
        await clienteRepository.Update(cliente);

        return NoContent();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
        {
            ModelState.AddModelError(nameof(id), "Id no válido.");
            return ValidationProblem(ModelState);
        }
        var cliente = await clienteRepository.GetAsync(id);

        if (cliente is null)
        {
            return NotFoundProblem("Cliente no existe");
        }

        var tieneCuentas = await cuentaRepository.AsQueryable().FirstOrDefaultResult(x => x.Cliente.Id == id);

        if(tieneCuentas is not null)
        {            
            return ForbiddenProblem("El Cliente tiene cuentas activas, eliminar primero.");
        }
        
        await clienteRepository.Delete(cliente);

        return NoContent();
    }
}
