using System.ComponentModel.DataAnnotations;
using System.Net;
using Core.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Middleware.ErrorHandling
{
    public class HttpExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<HttpExceptionHandlingMiddleware> _logger;

        public HttpExceptionHandlingMiddleware(ILogger<HttpExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (Exception ex)
            {
                var statusCode = GetErrorCode(ex);
                var message = ex.Message;

                _logger.LogError(ex, message);

                if (statusCode == HttpStatusCode.InternalServerError)
                {
                    message = "Internal Server Error.";
                }

                context.Response.StatusCode = (int)statusCode;
                await context.Response.WriteAsync(message);
            }
        }

        private HttpStatusCode GetErrorCode(Exception exception) =>
            exception switch
            {
                ValidationException => HttpStatusCode.BadRequest,
                HttpException httpException => httpException.StatusCode,
                _ => HttpStatusCode.InternalServerError,
            };
    }
}
