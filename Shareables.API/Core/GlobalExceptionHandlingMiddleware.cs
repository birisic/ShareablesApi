using Application;
using Application.Exceptions;
using FluentValidation;

namespace Shareables.API.Core
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private IExceptionLogger _logger;
        private IApplicationActor _actor;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, IExceptionLogger logger, IApplicationActor actor)
        {
            _next = next;
            _logger = logger;
            _actor = actor;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception exception)
            {
                if (exception is UnauthorizedAccessException)
                {
                    httpContext.Response.StatusCode = 401;
                    return;
                }

                if (exception is ValidationException ex)
                {
                    httpContext.Response.StatusCode = 422;
                    var body = ex.Errors.Select(x => new { Property = x.PropertyName, Error = x.ErrorMessage });

                    await httpContext.Response.WriteAsJsonAsync(body);
                    return;
                }

                if (exception is EntityNotFoundException)
                {
                    httpContext.Response.StatusCode = 404;
                    return;
                }

                if (exception is ConflictException c)
                {
                    httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
                    var body = new { error = c.Message };

                    await httpContext.Response.WriteAsJsonAsync(body);
                    return;
                }

                if (exception is ArgumentException a)
                {
                    httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    var body = new { error = a.Message };

                    await httpContext.Response.WriteAsJsonAsync(body);
                    return;
                }

                var errorId = _logger.Log(exception, _actor);

                httpContext.Response.StatusCode = 500;
                await httpContext.Response.WriteAsJsonAsync(new { Message = $"An unexpected error has occured. Please contact our support with this ID - {errorId}." });
            }
        }
    }

}
