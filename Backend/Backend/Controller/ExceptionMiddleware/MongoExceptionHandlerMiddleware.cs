using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;


namespace Backend.Controller;

public class MongoExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public MongoExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (MongoConnectionException ex)
        {
            await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, "Database connection error", ex);
        }
        catch (MongoCommandException ex)
        {
            await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, "Database command error", ex);
        }
        catch (MongoWriteException ex)
        {
            await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, "Database write error", ex);
        }
        catch (TimeoutException ex)
        {
            await HandleExceptionAsync(context, HttpStatusCode.RequestTimeout, "Database timeout", ex);
        }
        catch (FormatException ex)
        {
            await HandleExceptionAsync(context, HttpStatusCode.BadRequest, "Invalid ID format", ex);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, "Internal server error", ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string message, Exception ex)
    {
        Console.WriteLine($"❌ {message}: {ex.Message}");
        var response = new
        {
            status = (int)statusCode,
            error = message,
            details = ex.Message
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}