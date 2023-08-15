using Devsu.Api.Models;
using Devsu.Data;
using Devsu.Models;
using Microsoft.AspNetCore.Mvc;

namespace Devsu.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MovimientosController : BaseController
{
    private readonly IRepository<Cuenta> cuentaRepository;
    private readonly IRepository<Cliente> clienteRepository;
    private readonly IRepository<Movimiento> movimientoRepository;

    public MovimientosController(IRepository<Cuenta> cuentaRepository
        , IRepository<Cliente> clienteRepository
        , IRepository<Movimiento> movimientoRepository)
    {
        this.cuentaRepository = cuentaRepository ?? throw new ArgumentNullException(nameof(cuentaRepository));
        this.clienteRepository = clienteRepository ?? throw new ArgumentNullException(nameof(clienteRepository));
        this.movimientoRepository = movimientoRepository;
    }

    [HttpGet("")]
    public async Task<ActionResult> Get([FromQuery] Guid? cliente, int? cuenta)
    {
        var movimientos = Enumerable.Empty<Movimiento>();
        if (cliente.GetValueOrDefault() != Guid.Empty)
        {
            movimientos = await movimientoRepository.AsQueryable()
                .Where(x => x.Cuenta.Cliente.Id == cliente!.Value)
                .Results();
        }
        else
        {
            movimientos = await movimientoRepository.GetAllAsync();
        }

        if(cuenta.HasValue)
        {
            movimientos = movimientos.Where(x => x.Cuenta.Numero == cuenta.Value);
        }

        var models = movimientos.Select(x => MovimientoViewModel.From(x));
        return Ok(models);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> Get([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
        {
            ModelState.AddModelError(nameof(id), "Id no válido.");
            return ValidationProblem(ModelState);
        }
        var movimiento = await movimientoRepository.GetAsync(id);
        if (movimiento is null)
        {
            return NotFound("Movimiento no encontrado");
        }
        return Ok(MovimientoViewModel.From(movimiento));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MovimientoInputModel model)
    {
        if (!ModelState.IsValid)
        {            
            return ValidationProblem(ModelState);
        }

        var cliente = await clienteRepository.AsQueryable().Where(x => x.Nombre == model.Cliente).FirstOrDefaultResult();

        if (cliente is null)
        {
            return NotFoundProblem("Cliente no existe");            
        }

        var cuenta = await cuentaRepository.AsQueryable().Where(x => x.Cliente.Id == cliente.Id && x.Numero == model.CuentaNumero).FirstOrDefaultResult();

        if (cuenta is null)
        {
            return NotFoundProblem("Cuenta no existe");            
        }

        var ultimoMovimiento = await movimientoRepository.AsQueryable().OrderByDescending(x => x.Fecha).FirstOrDefaultResult();
        var saldo = ultimoMovimiento?.Saldo ?? cuenta.SaldoInicial;
        var tipoMovimiento = Enum.GetValues(typeof(TipoMovimiento))
                  .Cast<TipoMovimiento>()
                  .FirstOrDefault(x => x.ToString().ToLower()[0] == model.TipoMovimiento.ToLower()[0]);


        if (tipoMovimiento == TipoMovimiento.Debito && saldo < model.Valor)
        {
            ModelState.AddModelError(nameof(MovimientoInputModel.Valor), "Saldo no disponible.");
            return ValidationProblem(ModelState);
        }

        var debitosHoy = await movimientoRepository.AsQueryable()
            .Where(x => x.Cuenta.Id == cuenta.Id && x.TipoMovimiento == TipoMovimiento.Debito)
            .SumResults(x => x.Valor);

        if (tipoMovimiento == TipoMovimiento.Debito && debitosHoy + model.Valor > Movimiento.MaximoExtraccion)
        {            
            ModelState.AddModelError(nameof(MovimientoInputModel.Valor), "Cupo diario excedido.");
            return ValidationProblem(ModelState);
        }

        var id = Guid.NewGuid();
        var nuevoMovimiento = new Movimiento
        {
            Cuenta = cuenta,
            Id = id,
            Fecha = DateTime.Now,
            TipoMovimiento = tipoMovimiento,
            Valor = model.Valor,
            Saldo = saldo + (tipoMovimiento == TipoMovimiento.Credito ? model.Valor : model.Valor * -1)
        };
        await movimientoRepository.AddAsync(nuevoMovimiento);

        return Created(Url?.RouteUrl("", new { controller = "movimientos", action = "", id = id }) ?? "", MovimientoViewModel.From(nuevoMovimiento));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
        {
            ModelState.AddModelError(nameof(id), "Id no válido.");
            return ValidationProblem(ModelState);
        }

        var ultimoMovimiento = await movimientoRepository.AsQueryable().OrderByDescending(x => x.Fecha).FirstOrDefaultResult();

        //solo permito eliminar el último movimiento. para evitar que se rompa el saldo y tenga que recorrer todos los registros
        if(ultimoMovimiento?.Id != id)
        {            
            return ForbiddenProblem("No se permite eliminar el movimiento por no ser el último");
        }

        var movimiento = await movimientoRepository.GetAsync(id);

        if (movimiento is null)
        {
            return NotFoundProblem("Cuenta no existe");            
        }

        await movimientoRepository.Delete(movimiento);

        return NoContent();
    }
}
