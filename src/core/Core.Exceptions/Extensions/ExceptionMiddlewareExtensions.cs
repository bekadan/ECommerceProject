using Core.Exceptions.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Core.Exceptions.Extensions;

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseCoreExceptionHandling(this IApplicationBuilder app, bool includeDetails = false)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>(includeDetails);
    }
}

/*
 // Register Core.Logging first
builder.Services.AddCoreLogging(builder.Configuration);

var app = builder.Build();

// Use global exception handling
var isDev = app.Environment.IsDevelopment();
app.UseCoreExceptionHandling(includeDetails: isDev);
 
 */
