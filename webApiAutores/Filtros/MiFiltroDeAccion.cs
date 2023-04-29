using Microsoft.AspNetCore.Mvc.Filters;

namespace webAPIAutores.Filtros;

public class MiFiltroDeAccion : IActionFilter
{
    private readonly ILogger<MiFiltroDeAccion> loguer;

    public MiFiltroDeAccion(ILogger<MiFiltroDeAccion> _loguer)
    {
        loguer = _loguer;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        loguer.LogInformation("Antes de ejecutar la accion");
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        loguer.LogInformation("Despues de ejecutar la accion");
    }
}