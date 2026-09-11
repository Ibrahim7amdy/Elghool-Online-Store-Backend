using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, IHostEnvironment env)
        {
            _next = next;
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";


            var statusCode = HttpStatusCode.BadRequest;
            var message = exception.Message;

            if (exception is DbUpdateException ||
                exception is TimeoutException ||
                exception is OutOfMemoryException)
            {
                statusCode = HttpStatusCode.InternalServerError;
                message = "حدث خطأ غير متوقع في السيرفر.";
            }

            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = message,
                Details = _env.IsDevelopment() ? exception.StackTrace : null
            };

            var json = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(json);
        }
    }
}