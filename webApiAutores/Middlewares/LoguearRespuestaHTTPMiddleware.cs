namespace webAPIAutores.Middlewares;

public static class LoguearRespuestaHTTPMiddlewaresExtensions 
{
    public static IApplicationBuilder UseLoguearRespuestaHTTP(this IApplicationBuilder app) =>
        app.UseMiddleware<LoguearRespuestaHTTPMiddleware>();
}

public class LoguearRespuestaHTTPMiddleware
{
    private readonly RequestDelegate siguiente;
    private readonly ILogger<LoguearRespuestaHTTPMiddleware> logger;

    public LoguearRespuestaHTTPMiddleware(
        RequestDelegate _siguiente,
        ILogger<LoguearRespuestaHTTPMiddleware> _logger)
    {
        siguiente = _siguiente;
        logger = _logger;
    }

    public async Task InvokeAsync(HttpContext contexto)
    {
        using (var ms = new MemoryStream())
        {
            var cuerpoOriginalRespuesta = contexto.Response.Body;
            contexto.Response.Body = ms;

            await siguiente(contexto);

            ms.Seek(0, SeekOrigin.Begin);
            string respuesta = new StreamReader(ms).ReadToEnd();
            ms.Seek(0, SeekOrigin.Begin);

            await ms.CopyToAsync(cuerpoOriginalRespuesta);
            contexto.Response.Body = cuerpoOriginalRespuesta;

            logger.LogInformation(respuesta);
        }
    }
}