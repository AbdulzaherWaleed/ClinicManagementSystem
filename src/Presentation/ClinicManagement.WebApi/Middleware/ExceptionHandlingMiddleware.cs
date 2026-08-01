using System.Net;
using System.Text.Json;
using ClinicManagement.Application.Common.Exceptions;
using ClinicManagement.Domain.Exceptions;

namespace ClinicManagement.WebApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await WriteErrorResponseAsync(context, ex);
        }
    }

    private async Task WriteErrorResponseAsync(HttpContext context, Exception exception)
    {
        var (statusCode, payload) = MapException(exception);

        if (statusCode >= (int)HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, JsonOptions));
    }

    private (int StatusCode, object Payload) MapException(Exception exception)
    {
        return exception switch
        {
            ValidationException validation => (
                StatusCodes.Status400BadRequest,
                new { title = "Validation failed", errors = validation.Errors }),

            NotFoundException notFound => (
                StatusCodes.Status404NotFound,
                new { title = notFound.Message }),

            ForbiddenAccessException forbidden => (
                StatusCodes.Status403Forbidden,
                new { title = forbidden.Message }),

            DomainException domain => (
                StatusCodes.Status400BadRequest,
                new { title = domain.Message }),

            _ => (
                StatusCodes.Status500InternalServerError,
                new
                {
                    title = "An unexpected error occurred.",
                    detail = _environment.IsDevelopment() ? exception.Message : null
                })
        };
    }
}
