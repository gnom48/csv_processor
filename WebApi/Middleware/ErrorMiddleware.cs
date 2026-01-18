using CsvProcessor.Exceptions;

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
            HandleInternalException(context, ex);
        }
        catch (Exception ex)
        {
            HandleUnknownException(context, ex);
        }
    }

    private async void HandleInternalException(HttpContext context, BaseMsgException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsync($"{ex.GetType().Name}: {ex.Msg} ({ex.Message})");
    }

    private async void HandleUnknownException(HttpContext context, Exception ex)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsync($"Упс) неизвестная ошибка: {ex.GetType().Name} (содержимое ошибка см. логи)");
    }
}