using System;
using System.Net.Mime;
using System.Text.Json;

namespace portfolio.api;

public class ApiResponseExceptionMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BadHttpRequestException ex)
        {
            await HandleException(context, ex, StatusCodes.Status400BadRequest);
        }
        catch (HttpRequestException ex)
        {
            await HandleException(context, ex, (int)ex.StatusCode!);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex);
        }
    }

    static Task HandleException(HttpContext context, 
                                Exception exception,
                                int statusCode = StatusCodes.Status500InternalServerError)
    {
        context.Response.ContentType = MediaTypeNames.Application.Json;
        context.Response.StatusCode = statusCode;

        var response = new ApiResponse<object>
        {
            Success = false,
            Message = exception.Message + Environment.NewLine + exception.StackTrace,
        };

        var jsonResponse = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(jsonResponse);
    }
}
