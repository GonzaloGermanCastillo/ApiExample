using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Net.Http.Headers;

namespace Devsu.Api.Controllers;

public abstract class BaseController : ControllerBase
{
    protected IActionResult ForbiddenProblem(string message)
    {
        var pd = new ProblemDetails
        {
            Title = "Operación prohibida",
            Instance = Request?.Path ?? "",
            Status = StatusCodes.Status403Forbidden,
            Type = $"https://httpstatuses.com/403",
            Detail = message,
        };
        return new ObjectResult(pd) { StatusCode = StatusCodes.Status403Forbidden, ContentTypes = { MimeTypes.ProblemHeader } };
    }

    protected IActionResult NotFoundProblem(string message)
    {
        var pd = new ProblemDetails
        {
            Title = "Entidad no encontrada",
            Instance = Request?.Path ?? "",
            Status = StatusCodes.Status404NotFound,
            Type = $"https://httpstatuses.com/404",
            Detail = message,
        };
        return new ObjectResult(pd) { StatusCode = StatusCodes.Status404NotFound, ContentTypes = { MimeTypes.ProblemHeader } };
    }

    [NonAction]
    public override ActionResult ValidationProblem(ModelStateDictionary modelState)
    {
        return ValidationProblem(GetValidationProblem(modelState));
    }

    protected IActionResult ConflictProblem(ModelStateDictionary modelState)
    {
        return new ConflictObjectResult(GetConflictProblem(modelState)) { ContentTypes = { MimeTypes.ProblemHeader } };
    }

    protected ValidationProblemDetails GetValidationProblem(ModelStateDictionary modelState)
    {
        return new ValidationProblemDetails(modelState)
        {
            Title = "Validación de información esperada fallida",
            Instance = Request?.Path ?? "",
            Status = StatusCodes.Status400BadRequest,
            Type = $"https://httpstatuses.com/400",
            Detail = "Alguna información contiene valores no validos.",
        };
    }

    protected ValidationProblemDetails GetConflictProblem(ModelStateDictionary modelState)
    {
        return new ValidationProblemDetails(modelState)
        {
            Title = "Entidad duplicada",
            Instance = Request?.Path ?? "",
            Status = StatusCodes.Status409Conflict,
            Type = $"https://httpstatuses.com/409",
            Detail = "No es posible crear la información requerida porque generaría una duplicación no admitida.",
        };
    }    

    public class MimeTypes
    {
        // http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html
        public const string Entry = "application/vnd.autocosmos.entry+json";
        public const string Reference = "application/vnd.autocosmos.ref+json";
        public const string Extended = "application/vnd.autocosmos.ex-entry+json";
        public const string AppJson = "application/json";
        public const string TextJson = "text/json";
        public const string Problem = "application/problem+json";

        public static readonly MediaTypeHeaderValue EntryHeader = new MediaTypeHeaderValue(Entry);
        public static readonly MediaTypeHeaderValue ReferenceHeader = new MediaTypeHeaderValue(Reference);
        public static readonly MediaTypeHeaderValue ExtendedHeader = new MediaTypeHeaderValue(Extended);
        public static readonly MediaTypeHeaderValue ProblemHeader = new MediaTypeHeaderValue(Problem);

        public static string[] EntryGroup = new[]{
            AppJson,
            TextJson,
            Entry,
            };
    }
}
