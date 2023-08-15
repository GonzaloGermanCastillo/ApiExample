using Devsu.Api.Models;
using Devsu.Data;
using Devsu.Models;
using Microsoft.AspNetCore.Mvc;

namespace Devsu.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CuentasController : BaseController
{
    private readonly IRepository<Cuenta> cuentaRepository;
    private readonly IRepository<Cliente> clienteRepository;
    private readonly IRepository<Movimiento> movimientoRepository;

    public CuentasController(IRepository<Cuenta> cuentaRepository
        , IRepository<Cliente> clienteRepository
        , IRepository<Movimiento> movimientoRepository)
    {
        this.cuentaRepository = cuentaRepository ?? throw new ArgumentNullException(nameof(cuentaRepository));
        this.clienteRepository = clienteRepository ?? throw new ArgumentNullException(nameof(clienteRepository));
        this.movimientoRepository = movimientoRepository ?? throw new ArgumentNullException(nameof(movimientoRepository));
    }

    [HttpGet("")]
    public async Task<ActionResult> Get([FromQuery] Guid? cliente)
    {
        var cuentas = Enumerable.Empty<Cuenta>();
        if (cliente.GetValueOrDefault() != Guid.Empty)
        {
            cuentas = await cuentaRepository.AsQueryable()
                .Where(x => x.Cliente.Id == cliente!.Value)
                .Results();
        }
        else
        {
            cuentas = await cuentaRepository.GetAllAsync();
        }
        var models = cuentas.Select(x => CuentaViewModel.From(x));
        return Ok(models);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
        {
            ModelState.AddModelError(nameof(id), "Id no válido.");
            return ValidationProblem(ModelState);
        }
        var cuenta = await cuentaRepository.GetAsync(id);
        if (cuenta is null)
        {
            return NotFoundProblem("Cuenta no encontrado");
        }
        return Ok(CuentaViewModel.From(cuenta));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CuentaInputModel model)
    {
        if (!ModelState.IsValid)
        {            
            return ValidationProblem(ModelState);
        }

        var cliente = await clienteRepository.AsQueryable().FirstOrDefaultResult(x => x.Nombre == model.Cliente);

        if (cliente is null)
        {
            return NotFoundProblem("Cliente no encontrado");
        }

        var existeCuenta = await cuentaRepository.AsQueryable().FirstOrDefaultResult(x => x.Numero == model.Numero);

        if (existeCuenta is not null)
        {
            ModelState.AddModelError(nameof(CuentaInputModel.Numero), "Número cuenta utilizado.");
            return ConflictProblem(ModelState);
        }

        var id = Guid.NewGuid();
        var tipoCuenta = Enum.GetValues(typeof(TipoCuenta))
                  .Cast<TipoCuenta>()
                  .FirstOrDefault(x => x.ToString().ToLower() == model.TipoCuenta.ToLower());

        var nuevaCuenta = new Cuenta
        {
            Cliente = cliente,
            Id = id,
            Numero = model.Numero,
            SaldoInicial = model.SaldoInicial,
            TipoCuenta = tipoCuenta
        };
        await cuentaRepository.AddAsync(nuevaCuenta);

        return Created(Url.RouteUrl("", new { controller = "cuentas", action = "", id = id }) ?? "", CuentaViewModel.From(nuevaCuenta));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] CuentaInputModel model)
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

        var cuenta = await cuentaRepository.GetAsync(id);

        if (cuenta is null)
        {
            return NotFoundProblem("Cuenta no existe");
        }

        var cliente = await clienteRepository.AsQueryable().FirstOrDefaultResult(x => x.Nombre == model.Cliente);

        if (cliente is null)
        {            
            return NotFoundProblem("Cliente no existe");
        }

        var tipoCuenta = Enum.GetValues(typeof(TipoCuenta))
                  .Cast<TipoCuenta>()
                  .FirstOrDefault(x => x.ToString().ToLower() == model.TipoCuenta.ToLower());

        cuenta.Cliente = cliente;
        cuenta.Numero = model.Numero;
        cuenta.SaldoInicial = model.SaldoInicial;
        cuenta.TipoCuenta = tipoCuenta;

        await cuentaRepository.Update(cuenta);

        return Ok(CuentaViewModel.From(cuenta));
    }

    [HttpPatch("{id}/Desactivar")]
    public async Task<IActionResult> Desactivar([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
        {            
            return ValidationProblem(ModelState);
        }
        var cuenta = await cuentaRepository.GetAsync(id);

        if (cuenta is null)
        {
            return NotFoundProblem("Cuenta no existe");
        }

        cuenta.Estado = false;
        await cuentaRepository.Update(cuenta);

        return NoContent();
    }

    [HttpPatch("{id}/Activar")]
    public async Task<IActionResult> Activar([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
        {            
            return ValidationProblem(ModelState);
        }
        var cuenta = await cuentaRepository.GetAsync(id);

        if (cuenta is null)
        {
            return NotFoundProblem("Cuenta no existe");
        }

        cuenta.Estado = true;
        await cuentaRepository.Update(cuenta);

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
        var cuenta = await cuentaRepository.GetAsync(id);

        if (cuenta is null)
        {            
            return NotFoundProblem("Cuenta no existe");
        }

        var existeMovimiento = await movimientoRepository.AsQueryable().FirstOrDefaultResult(x => x.Cuenta.Id == id);

        if(existeMovimiento is not null)            
        {
            return ForbiddenProblem("La cuenta tiene movimientos, eliminar primero.");
        }

        await cuentaRepository.Delete(cuenta);

        return NoContent();
    }

}
