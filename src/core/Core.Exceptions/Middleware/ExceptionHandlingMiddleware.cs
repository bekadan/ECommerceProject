using Core.Exceptions.Types;
using Core.Logging.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace Core.Exceptions.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;
    private readonly bool _includeDetails;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger logger, bool includeDetails = false)
    {
        _next = next;
        _logger = logger;
        _includeDetails = includeDetails;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var statusCode = MapToStatusCode(exception);
        context.Response.StatusCode = statusCode;

        // Correlation ID
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault() ?? Guid.NewGuid().ToString();

        // Structured logging with Serilog / Core.Logging
        var logProperties = new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["RequestPath"] = context.Request.Path,
            ["RequestMethod"] = context.Request.Method,
            ["StatusCode"] = statusCode,
            ["ExceptionType"] = exception.GetType().Name
        };

        if (_includeDetails)
            logProperties["ExceptionDetails"] = GetExceptionDetails(exception);

        // Log as critical for unhandled, or error for AppExceptions
        if (exception is AppException)
            _logger.Error(exception, "Handled exception {@Properties}", logProperties);
        else
            _logger.Critical(exception, "Unhandled exception {@Properties}", logProperties);

        // Prepare structured response
        var response = new
        {
            error = exception.Message,
            statusCode,
            correlationId,
            details = _includeDetails ? GetExceptionDetails(exception) : null
        };

        var result = JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });
        await context.Response.WriteAsync(result);
    }

    private int MapToStatusCode(Exception ex)
    {
        return ex switch
        {
            DomainException => 400,
            ValidationException => 400,
            NotFoundException => 404,
            UnauthorizedException => 401,
            ForbiddenException => 403,
            InternalServerException => 500,
            ExternalServiceException => 502,
            AppException appEx => appEx.StatusCode,
            _ => 500
        };
    }

    private object GetExceptionDetails(Exception ex)
    {
        return new
        {
            ex.Message,
            ex.StackTrace,
            inner = ex.InnerException != null ? GetExceptionDetails(ex.InnerException) : null
        };
    }
}
