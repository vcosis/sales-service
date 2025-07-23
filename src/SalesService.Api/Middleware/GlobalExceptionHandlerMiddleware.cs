using System.Text.Json;

namespace SalesService.Api.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new
        {
            type = GetErrorType(exception),
            error = GetErrorMessage(exception),
            detail = exception.Message,
            timestamp = DateTime.UtcNow
        };

        switch (exception)
        {
            case ArgumentException:
                response.StatusCode = 400; // Bad Request
                break;
            case InvalidOperationException:
                response.StatusCode = 400; // Bad Request
                break;
            case KeyNotFoundException:
                response.StatusCode = 404; // Not Found
                break;
            default:
                response.StatusCode = 500; // Internal Server Error
                _logger.LogError(exception, "Unhandled exception occurred");
                break;
        }

        await response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }

    private static string GetErrorType(Exception exception)
    {
        return exception switch
        {
            ArgumentException => "ValidationError",
            InvalidOperationException => "BusinessRuleViolation",
            KeyNotFoundException => "ResourceNotFound",
            _ => "InternalServerError"
        };
    }

    private static string GetErrorMessage(Exception exception)
    {
        return exception switch
        {
            ArgumentException => "Dados inválidos",
            InvalidOperationException => "Regra de negócio violada",
            KeyNotFoundException => "Recurso não encontrado",
            _ => "Erro interno do servidor"
        };
    }
} 