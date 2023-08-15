using Devsu.Api.Controllers;
using Devsu.Api.Tests.Mocks;
using Devsu.Models;
using Microsoft.AspNetCore.Mvc;
using SharpTestsEx;

namespace Devsu.Api.Tests;

public class MovimientosControllerTests
{
    [Test]
    public async Task CreateDebito_Created()
    {
        var cliente = new Cliente
        {
            Id = Guid.NewGuid(),
            Nombre = "Jose",
            Direccion = "Bolivar 12",
            Telefono = "1564123",
            Contrasenia = 7458,
        };

        var cuenta = new Cuenta
        {
            Id = Guid.NewGuid(),
            Cliente = cliente,
            Numero = 1,
            SaldoInicial = 5000,
            TipoCuenta = TipoCuenta.Ahorros
        };

        var controller = new MovimientosController(new CuentaRepositoryMock(cuenta), new ClienteRepositoryMock(cliente), new MovimientosRepositoryMock());
        var response = await controller.Create(new Models.MovimientoInputModel
        {
            Cliente = "Jose",
            CuentaNumero = 1,
            TipoMovimiento = "Debito",
            Valor = 500
        });

        var createdResult = response as CreatedResult;
        createdResult.Should().Not.Be.Null();
        createdResult?.StatusCode.Should().Be(201);        
    }

    [Test]
    public async Task CreateDebito_SinSaldo()
    {
        var cliente = new Cliente
        {
            Id = Guid.NewGuid(),
            Nombre = "Jose",
            Direccion = "Bolivar 12",
            Telefono = "1564123",
            Contrasenia = 7458,            
        };

        var cuenta = new Cuenta
        {
            Id = Guid.NewGuid(),
            Cliente = cliente,
            Numero = 1,
            SaldoInicial = 0,
            TipoCuenta = TipoCuenta.Ahorros
        };

        var controller = new MovimientosController(new CuentaRepositoryMock(cuenta), new ClienteRepositoryMock(cliente), new MovimientosRepositoryMock());
        var response = await controller.Create(new Models.MovimientoInputModel
        {
            Cliente = "Jose",
            CuentaNumero = 1,
            TipoMovimiento = "Debito",
            Valor = 500
        });

        var badRequest = response as BadRequestObjectResult;
        badRequest.Should().Not.Be.Null();
        badRequest?.StatusCode.Should().Be(400);
        var detalles = badRequest?.Value as ValidationProblemDetails;
        detalles.Should().Not.Be.Null();
        var firstError = detalles?.Errors?.FirstOrDefault();
        firstError.Should().Not.Be.Null();
        if (firstError != null)
        {
            var textoError = firstError.Value.Value.First();
            textoError.Should().Be("Saldo no disponible.");
        }
    }

    [Test]
    public async Task CreateDebito_TopeSuperado()
    {
        var cliente = new Cliente
        {
            Id = Guid.NewGuid(),
            Nombre = "Jose",
            Direccion = "Bolivar 12",
            Telefono = "1564123",
            Contrasenia = 7458,
        };

        var cuenta = new Cuenta
        {
            Id = Guid.NewGuid(),
            Cliente = cliente,
            Numero = 1,
            SaldoInicial = 5000,
            TipoCuenta = TipoCuenta.Ahorros
        };

        var primerMovimiento = new Movimiento
        {
            Id = Guid.NewGuid(),
            Cuenta = cuenta,
            Fecha = DateTime.Now,
            Saldo = 4200,
            TipoMovimiento = TipoMovimiento.Debito,
            Valor = 800
        };

        var controller = new MovimientosController(new CuentaRepositoryMock(cuenta), new ClienteRepositoryMock(cliente), new MovimientosRepositoryMock(primerMovimiento));
        var response = await controller.Create(new Models.MovimientoInputModel
        {
            Cliente = "Jose",
            CuentaNumero = 1,
            TipoMovimiento = "Debito",
            Valor = 500
        });

        var badRequest = response as BadRequestObjectResult;
        badRequest.Should().Not.Be.Null();
        badRequest?.StatusCode.Should().Be(400);
        var detalles = badRequest?.Value as ValidationProblemDetails;
        detalles.Should().Not.Be.Null();
        var firstError = detalles?.Errors?.FirstOrDefault();
        firstError.Should().Not.Be.Null();
        if (firstError != null)
        {
            var textoError = firstError.Value.Value.First();
            textoError.Should().Be("Cupo diario excedido.");
        }
    }
}