using System.Net;
using System.Text.Json;

namespace Truestory.WebAPI.Middlewares;

/// <summary>
/// Middleware for global exception handling in the application pipeline.
/// Catches unhandled exceptions during request processing and returns a consistent JSON error response.
/// </summary>
/// <param name="next">The next middleware in the pipeline.</param>
/// <param name="logger">Logger instance to log unhandled exceptions.</param>
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    /// <summary>
    /// Processes incoming HTTP requests and handles any uncaught exceptions that occur during the pipeline execution.
    /// Returns a standardized JSON error response to the client.
    /// </summary>
    /// <param name="context">The context for the current HTTP request.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occurred while processing request: {Path}", context.Request.Path);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new
            {
                error = "An unexpected error occurred.",
                message = ex.Message
            };

            var result = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(result);
        }
    }
}