using System.Diagnostics;
using Users.Application.Exceptions;
using Users.Api.Models;
using Users.Infrastructure.Exceptions;

namespace Users.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        var (statusCode, response) = exception switch
        {
            ValidationException validationEx => (
                StatusCodes.Status400BadRequest,
                new ErrorResponse(true, "Validation failed", validationEx.Errors, traceId)
            ),
            BadRequestException badRequest => (
                StatusCodes.Status400BadRequest,
                new ErrorResponse(true, badRequest.Message, null, traceId)
            ),
            NotFoundException notFound => (
                StatusCodes.Status404NotFound,
                new ErrorResponse(true, notFound.Message, null, traceId)
            ),
            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                new ErrorResponse(true, "Unauthorized", null, traceId)
            ),
            EmailConfigurationException emailConfigEx => (
                StatusCodes.Status500InternalServerError,
                new ErrorResponse(true, "Email service configuration error", null, traceId)
            ),
            EmailSendingException emailSendEx => (
                StatusCodes.Status500InternalServerError,
                new ErrorResponse(true, "Failed to send email", null, traceId)
            ),
            EmailException emailEx => (
                StatusCodes.Status500InternalServerError,
                new ErrorResponse(true, "Email service error", null, traceId)
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                new ErrorResponse(true, "An unexpected error occurred.", null, traceId)
            )
        };

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(response);
    }
}