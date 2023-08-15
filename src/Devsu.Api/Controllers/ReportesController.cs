using Devsu.Api.Models;
using Devsu.Data;
using Devsu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devsu.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportesController : BaseController
{
    private readonly IRepository<Movimiento> movimientoRepository;

    public ReportesController(IRepository<Movimiento> movimientoRepository)
    {
        this.movimientoRepository = movimientoRepository ?? throw new ArgumentNullException(nameof(movimientoRepository));
    }

    [HttpGet]
    public async Task<IActionResult> Reporte(Guid cliente, string fecha)
    {
        if (cliente == Guid.Empty)
        {
            ModelState.AddModelError(nameof(cliente), "ClienteId no válido.");
            return ValidationProblem(ModelState);
        }

        var validacion = validaFechas();
        if (!validacion.rangoValido)
        {
            ModelState.AddModelError(nameof(fecha), "Rango fechas inválido (Ej. fechaDesde.fechaHasta)");
            return ValidationProblem(ModelState);            
        }

        var movimientos = await movimientoRepository.AsQueryable()
            .Where(x => x.Cuenta.Cliente.Id == cliente && x.Fecha >= validacion.desde!.Value && x.Fecha < validacion.hasta!.Value.AddDays(1))
            .ToListAsync();

        return Ok(movimientos.Select(x => ReporteViewModel.From(x)));

        (bool rangoValido, DateTime? desde, DateTime? hasta) validaFechas()
        {
            /*
             * Ej. Formato válido: 2022-10-01.2022-10-31
             */
            if(string.IsNullOrEmpty(fecha) || !fecha.Contains('.'))
            {
                return (false, null, null);
            }

            var fechas = fecha.Split('.');
            if (!DateTime.TryParse(fechas[0], out var desde) || !DateTime.TryParse(fechas[1], out var hasta))
            {
                return (false, null, null);
            }

            if(desde > hasta)
            {
                return (false, null, null);
            }

            return (true, desde, hasta);
        }
    }
}
