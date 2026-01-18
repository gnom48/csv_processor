using System.Text.Json;
using CsvProcessor.Exceptions;
using WebApi.Models.Dto;

namespace WebApi.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BaseMsgException ex)
        {
            await HandleInternalException(context, ex);
        }
        catch (Exception ex)
        {
            await HandleUnknownException(context, ex);
        }
    }

    private async Task HandleInternalException(HttpContext context, BaseMsgException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorResponseDto
        {
            Msg = $"{ex.GetType().Name}: {ex.Msg}",
            Detail = ex.Message
        }));
    }

    private async Task HandleUnknownException(HttpContext context, Exception ex)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorResponseDto
        {
            Msg = $"Упс) неизвестная ошибка: {ex.GetType().Name} (содержимое ошибка см. логи)"
        }));
    }
}