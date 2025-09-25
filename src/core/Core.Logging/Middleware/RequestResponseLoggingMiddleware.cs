using Core.Logging.Abstractions;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Core.Logging.Middleware;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestResponseLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Log Request
        context.Request.EnableBuffering();
        string requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
        context.Request.Body.Position = 0;

        Log.Information("Incoming Request: {Method} {Path} {Body}",
            context.Request.Method, context.Request.Path, requestBody);

        // Capture Response
        var originalBody = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        string responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        Log.Information("Outgoing Response: {StatusCode} {Body}", context.Response.StatusCode, responseText);

        await responseBody.CopyToAsync(originalBody);
    }
}
