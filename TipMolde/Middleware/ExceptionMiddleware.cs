using Microsoft.AspNetCore.Mvc;

namespace TipMolde.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro nao tratado. TraceId: {TraceId}", context.TraceIdentifier);

                var (status, title) = ex switch
                {
                    ArgumentException => (StatusCodes.Status400BadRequest, "Pedido invalido"),
                    KeyNotFoundException => (StatusCodes.Status404NotFound, "Recurso nao encontrado"),
                    UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Nao autorizado"),
                    _ => (StatusCodes.Status500InternalServerError, "Erro interno")
                };

                var detail = status >= 500 && !_env.IsDevelopment()
                    ? "Ocorreu um erro ao processar o pedido."
                    : ex.Message;

                var problem = new ProblemDetails
                {
                    Status = status,
                    Title = title,
                    Detail = detail,
                    Instance = context.Request.Path,
                    Type = $"https://httpstatuses.com/{status}"
                };

                problem.Extensions["traceId"] = context.TraceIdentifier;

                context.Response.StatusCode = status;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(problem);
            }
        }
    }
}
