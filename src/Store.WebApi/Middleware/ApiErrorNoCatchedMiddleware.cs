using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Store.ApplicationCore.ResponsesApi.Errors;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Store.WebApi.Middleware;
public class ApiErrorNoCatchedMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiErrorNoCatchedMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ApiErrorNoCatchedMiddleware(RequestDelegate next,
        ILogger<ApiErrorNoCatchedMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var statusCode = (int)HttpStatusCode.InternalServerError;

            _logger.LogError(ex, ex.Message);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = _env.IsDevelopment()
                            ? new ApiErrorNoCatched(statusCode, ex.Message, ex.StackTrace.ToString())
                            : new ApiErrorNoCatched(statusCode);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var json = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
        }
    }

}
