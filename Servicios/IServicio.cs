namespace webAPIAutores.Servicios;

public interface IServicio
{
    void RealizarTarea();
}

public class ServicioA : IServicio
{
    public void RealizarTarea()
    {
        throw new NotImplementedException();
    }
}

public class ServicioB : IServicio
{
    public void RealizarTarea()
    {
        throw new NotImplementedException();
    }
}