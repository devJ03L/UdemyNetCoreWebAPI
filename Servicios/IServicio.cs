namespace webAPIAutores.Servicios;

public interface IServicio
{
    void RealizarTarea();
    Guid ObtenerTransient();
    Guid ObtenerScoped();
    Guid ObtenerSingleton();
}

public class ServicioA : IServicio
{
    private readonly ILogger<ServicioA> logger;
    private readonly ServicioTransient servicioTransient;
    private readonly ServicioScoped servicioScoped;
    private readonly ServicioSingleton servicioSingleton;

    public ServicioA(
        ILogger<ServicioA> _logger,
        ServicioTransient _servicioTransient,
        ServicioScoped _servicioScoped,
        ServicioSingleton _servicioSingleton)
    {
        logger = _logger;
        servicioTransient = _servicioTransient;
        servicioScoped = _servicioScoped;
        servicioSingleton = _servicioSingleton;
    }

    public Guid ObtenerTransient() => servicioTransient.Guid;
    public Guid ObtenerScoped() => servicioScoped.Guid;
    public Guid ObtenerSingleton() => servicioSingleton.Guid;
    public void RealizarTarea()
    {
        throw new NotImplementedException();
    }
}

public class ServicioB : IServicio
{
    public Guid ObtenerScoped()
    {
        throw new NotImplementedException();
    }

    public Guid ObtenerSingleton()
    {
        throw new NotImplementedException();
    }

    public Guid ObtenerTransient()
    {
        throw new NotImplementedException();
    }

    public void RealizarTarea()
    {
        throw new NotImplementedException();
    }
}

public class ServicioTransient
{
    public Guid Guid = Guid.NewGuid();
}

public class ServicioScoped
{
    public Guid Guid = Guid.NewGuid();
}

public class ServicioSingleton
{
    public Guid Guid = Guid.NewGuid();
}