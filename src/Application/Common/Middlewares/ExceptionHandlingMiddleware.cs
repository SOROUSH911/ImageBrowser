using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ImageBrowser.Application.Common.Middlewares;
public class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Application.Common.Exceptions.ValidationException ex)
        {
            _logger.LogWarning("Validation failed: {ValidationErrors}", ex.Errors);

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            // Convert validation errors to a JSON-friendly representation
            var errorResponse = new
            {
                errors = ex.Errors.Select(e => new { field = e.Key, message = e.Value})
            };

            await context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
        }
        catch (Exception ex)
        {
            // Handle other exceptions as needed (e.g., log, return generic error response)
            _logger.LogError(ex, "An unexpected error occurred.");
        }
    }
}
