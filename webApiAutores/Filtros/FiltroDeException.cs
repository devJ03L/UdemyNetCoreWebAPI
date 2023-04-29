using Microsoft.AspNetCore.Mvc.Filters;

namespace webAPIAutores.Filtros;

public class FiltroDeException : ExceptionFilterAttribute
{
    private readonly ILogger<FiltroDeException> loguer;

    public FiltroDeException(ILogger<FiltroDeException> _loguer)
    {
        loguer = _loguer;
    }

    public override void OnException(ExceptionContext context)
    {
        loguer.LogError(context.Exception, context.Exception.Message);
        base.OnException(context);
    }
}